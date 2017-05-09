using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace scaling_microservices.Auth.Tokens
{
    public class RedisTokenStorage
    {
        ConnectionMultiplexer redis;
        IDatabase idDb;
        IDatabase tokenDb;
        IDatabase roleDb;

        public RedisTokenStorage(string configuration = "localhost", int dbIndex = 0, int dbMirrorIndex = 1, int dbRolesIndex = 2)
        {
            redis = ConnectionMultiplexer.Connect(configuration);
            idDb = redis.GetDatabase(dbIndex);
            tokenDb = redis.GetDatabase(dbMirrorIndex);
            roleDb = redis.GetDatabase(dbRolesIndex);
        }

        public async Task<bool> Set(int id, string token, string[] roles = null, int? timeInSeconds = null)
        {
            return await Set(id.ToString(), token, roles, timeInSeconds);
        }
        public async Task<bool> Set(string id, string token, string[] roles = null, int? timeInSeconds = null)
        {
            TimeSpan? expiry = null;
            if(timeInSeconds.HasValue)
            {
                expiry = TimeSpan.FromSeconds(timeInSeconds.Value);
            }
            var idTask = idDb.StringSetAsync(id, token, expiry);
            var tokenTask = tokenDb.StringSetAsync(token, id, expiry);
            var roleTask = roleDb.StringSetAsync(token, JsonConvert.SerializeObject(roles), expiry);
            await Task.WhenAll(new Task[] { idTask, tokenTask, roleTask });
            return (idTask.Result && tokenTask.Result && roleTask.Result);
        }

        public async Task<string> Get(int id)
        {
            return await Get(id.ToString());
        }
        public async Task<string> Get(string id)
        {
            var keyTask = idDb.StringGetAsync(id);
            await keyTask;
            var result =  keyTask.Result;
            if (result.HasValue)
                return result.ToString();
            else return null;
        }

        public async Task<string> GetId(string token)
        {
            var idTask = tokenDb.StringGetAsync(token);
            await idTask;
            var result = idTask.Result;
            if (result.HasValue)
                return result.ToString();
            else return null;
        }

        public async Task<string[]> GetRoles(string token)
        {
            var roleTask = roleDb.StringGetAsync(token);
            await roleTask;
            var result = roleTask.Result;
            if (result.HasValue)
                return JsonConvert.DeserializeObject<string[]>(result.ToString());
            else return null;
        }


        #region Delete
        private async Task<bool> DeleteString(string id, string token)
        {
            var tokenTask = tokenDb.KeyDeleteAsync(token);
            var idTask = idDb.KeyDeleteAsync(id);
            var roleTask = roleDb.KeyDeleteAsync(token);
            await Task.WhenAll(new Task[] { idTask, tokenTask, roleTask });
            return idTask.Result && tokenTask.Result;
        }

        public async Task<bool> DeleteKey(string key)
        {
            var nullOrValue = idDb.StringGet(key);
            string token = "";
            if(nullOrValue.HasValue)
            {
                token = nullOrValue.ToString();
            }
            else
            {
                return false;
            }
            return await DeleteString(key, token);
        }

        public async Task<bool> DeleteValue(string token)
        {

            var nullOrValue = idDb.StringGet(token);
            string key = "";
            if (nullOrValue.HasValue)
            {
                key = nullOrValue.ToString();
            }
            else
            {
                return false;
            }
            return await DeleteString(key, token);
        }
        #endregion
        #region Update
        private async Task<bool> UpdateExpiry(string id, string token, int timeInSeconds)
        {
            var tokenTask = tokenDb.KeyExpireAsync(token, DateTime.Now.AddSeconds(timeInSeconds));
            var idTask = idDb.KeyExpireAsync(id, DateTime.Now.AddSeconds(timeInSeconds));
            var roleTask = roleDb.KeyExpireAsync(token, DateTime.Now.AddSeconds(timeInSeconds));
            await Task.WhenAll(new Task[] { tokenTask, idTask, roleTask});
            return idTask.Result && tokenTask.Result && roleTask.Result;
        }

        public async Task<bool> UpdateExpiryById(int id, int timeInSeconds = 3600)
        {
            return await UpdateExpiryById(id.ToString(), timeInSeconds);
        }

        public async Task<bool> UpdateExpiryById(string id, int timeInSeconds = 3600)
        {
            var token = Get(id).Result;
            if (token == null)
            {
                return false;
            }
            return await UpdateExpiry(id, token, timeInSeconds);
        }

        public async Task<bool> UpdateExpiryByToken(string token, int timeInSeconds = 3600)
        {
            var id = GetId(token).Result;
            if(id == null)
            {
                return false;
            }
            return await UpdateExpiry(id, token, timeInSeconds);
        }
        #endregion
        public async Task<TimeSpan?> TTLById(int id)
        {
            return await TTLById(id.ToString());
        }
        public async Task<TimeSpan?> TTLById(string id)
        {
            return await idDb.KeyTimeToLiveAsync(id);
        }

        public async Task<TimeSpan?> TTLByToken(string token)
        {
            return await tokenDb.KeyTimeToLiveAsync(token);
        }
    }
}
