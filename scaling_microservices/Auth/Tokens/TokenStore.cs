using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace scaling_microservices.Auth.Tokens
{
    public class TokenStore : ITokenStore
    {
        const int ExpiryTimeOffset = 3600;

        RedisKeyValueStorage storage;

        public TokenStore()
        {
            storage = new RedisKeyValueStorage();
        }

        public TokenEntity GenerateToken(int userId)
        {
            Task<bool> task;
            string tokenData = storage.Get(userId.ToString()).Result;
            DateTime issueTime = DateTime.Now;
            DateTime expiryTime = DateTime.Now.AddSeconds(ExpiryTimeOffset);
            var token = new TokenEntity() {
                UserId = userId,
                ExpiresOn = expiryTime,
                IssuedOn = issueTime };
            if(tokenData != null)
            {
                token.AuthToken = tokenData;
                task = storage.UpdateKeyExpiry(userId.ToString());
            }
            else
            {
                token.AuthToken = Guid.NewGuid().ToString();
                task = storage.Set(userId.ToString(), token.AuthToken, ExpiryTimeOffset);
            }
            if(!task.Result)
            {
                throw new Exception("Could not set token");
            }
            return new TokenEntity(token);
        }

        public bool RemoveUser(int userId)
        {
            return storage.DeleteKey(userId.ToString()).Result;
        }

        public bool RemoveToken(string token)
        {
            return storage.DeleteValue(token).Result;
        }

        public bool ValidateToken(string tokenId)
        {
            return storage.UpdateValueExpiry(tokenId).Result;
        }
    }
}
