using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using StackExchange.Redis;

namespace scaling_microservices.Auth.Tokens
{
    public class RedisKeyValueStorage
    {
        ConnectionMultiplexer redis;
        IDatabase keyDb;
        IDatabase valueDb;

        public RedisKeyValueStorage(string configuration = "localhost", int dbIndex = 0, int dbMirrorIndex = 1)
        {
            redis = ConnectionMultiplexer.Connect(configuration);
            keyDb = redis.GetDatabase(dbIndex);
            valueDb = redis.GetDatabase(dbMirrorIndex);
        }
        
        public async Task<bool> Set(string key, string value, int? timeInSeconds = null)
        {
            TimeSpan? expiry = null;
            if(timeInSeconds.HasValue)
            {
                expiry = TimeSpan.FromSeconds(timeInSeconds.Value);
            }
            var keyTask = keyDb.StringSetAsync(key, value, expiry);
            var valueTask = valueDb.StringSetAsync(value, key, expiry);
            await Task.WhenAll(new Task[] { keyTask, valueTask });
            return (keyTask.Result && valueTask.Result);
        }

        public async Task<string> Get(string key)
        {
            var keyTask = keyDb.StringGetAsync(key);
            await keyTask;
            var result =  keyTask.Result;
            if (result.HasValue)
                return result.ToString();
            else return null;
        }


        #region Delete
        private static Task<bool> DeleteString(string toDelete, IDatabase target, IDatabase mirror)
        {
            var nullOrValue = target.StringGet(toDelete);
            string mirrorDelete = null;
            if (nullOrValue.HasValue)
                mirrorDelete = nullOrValue.ToString();
            else
                return new Task<bool>(() => false);
            var mirrorTask = mirror.KeyDeleteAsync(mirrorDelete);
            var targetTask = target.KeyDeleteAsync(toDelete);
            return new Task<bool>(() =>
            {
               Task.WhenAll(new Task[] { targetTask, mirrorTask }).Wait();
               return targetTask.Result && mirrorTask.Result;
            });
        }

        public async Task<bool> DeleteKey(string key)
        {
            var res = DeleteString(key, keyDb, valueDb);
            res.Start();
            return await res;
        }

        public async Task<bool> DeleteValue(string value)
        {
            var res = DeleteString(value, valueDb, keyDb);
            res.Start();
            return await res;
        }
        #endregion
        #region Update
        private static Task<bool> UpdateExpiry(string entry, IDatabase target, IDatabase mirror, int timeInSeconds)
        {
            var nullOrValue = target.StringGet(entry);
            string mirrorKey = null;
            Task<bool> result;
            if (nullOrValue.HasValue)
                mirrorKey = nullOrValue.ToString();
            else
            {
                result = new Task<bool>(() => false);
                result.Start();
                return result;
            }
            var mirrorTask = mirror.KeyExpireAsync(mirrorKey, DateTime.Now.AddSeconds(timeInSeconds));
            var targetTask = target.KeyExpireAsync(entry, DateTime.Now.AddSeconds(timeInSeconds));
            result = new Task<bool>(() =>
            {
                Task.WhenAll(new Task[] { mirrorTask, targetTask }).Wait();
                return targetTask.Result && mirrorTask.Result;
            });
            result.Start();
            return result;
        }

        public async Task<bool> UpdateKeyExpiry(string key, int timeInSeconds = 3600)
        {
            return await UpdateExpiry(key, keyDb, valueDb, timeInSeconds);
            
        }

        public async Task<bool> UpdateValueExpiry(string value, int timeInSeconds = 3600)
        {
            return await UpdateExpiry(value, valueDb, keyDb, timeInSeconds);
        }
        #endregion

        public async Task<TimeSpan?> KeyTTL(string key)
        {
            return await keyDb.KeyTimeToLiveAsync(key);
        }

        public async Task<TimeSpan?> ValueTTL(string value)
        {
            return await valueDb.KeyTimeToLiveAsync(value);
        }
    }
}
