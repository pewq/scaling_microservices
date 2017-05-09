using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace scaling_microservices.Auth.Tokens
{
    public class TokenStore : ITokenStore
    {
        const int ExpiryTimeOffset = 3600;

        RedisTokenStorage storage;

        public TokenStore()
        {
            storage = new RedisTokenStorage();
        }


        public bool Add(string userId, string token, string[] roles = null, int offset = ExpiryTimeOffset)
        {
            return storage.Set(userId, token, roles, offset).Result;
        }

        public TokenEntity GenerateToken(int userId, string[] roles = null)
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
                task = storage.UpdateExpiryById(userId.ToString());
            }
            else
            {
                token.AuthToken = Guid.NewGuid().ToString();
                task = storage.Set(userId, token.AuthToken, roles, ExpiryTimeOffset);
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
            return storage.UpdateExpiryByToken(tokenId).Result;
        }
    }
}
