using System;
using scaling_microservices.Rabbit;
using scaling_microservices.Auth.Tokens;

namespace auth_service
{
    public class AuthService : IService
    {
        TokenStore storage = new TokenStore();
        public AuthService() : base()
        {
            ThisInit();
        }

        public AuthService(string queueName) : base(queueName)
        {
            ThisInit();
        }



        private void ThisInit()
        {
            //Init db here;
            this.Handlers.Add("authenticate", (RequestHandleDelegate)AuthenticationHandler);
            this.Handlers.Add("authorize", (RequestHandleDelegate)AuthorizationHandler);
            this.Handlers.Add("validate", (RequestHandleDelegate)ValidateTokenHandler);
        }

        private int handleBasicAuth(string token)
        {
            return 1;
        }

        private TokenEntity handleCredentialBasicAuth(string login, string password)
        {
            //Get user id by login and password
            //or return null
            return storage.GenerateToken(login.GetHashCode());
        }

        private int handleCredentialAuth(string login, string password)
        {
            return 1;
        }

        private void AuthenticationHandler(QueueRequest req)
        {
            try
            {
                string authType = req["type"];
                TokenEntity userToken = null;
                switch(authType.ToLower())
                {
                    case "token":
                        {
                            //handle available token
                            break;
                        }
                    case "basic":
                        {
                            userToken = handleCredentialBasicAuth(req["login"], req["password"]);
                            break;
                        }
                    case "credential":
                        {
                           // userToken = handleCredentialAuth(req["login"], req["password"]);
                            break;
                        }
                    default:
                        {
                            throw new Exception("Invalid authentication type");
                        }
                }
                OnResponse(req.properties, userToken);
            }
            catch(Exception e)
            {
                OnException(e, req);
            }
        }

        private void AuthorizationHandler(QueueRequest req)
        {
            try
            {
                bool success = storage.ValidateToken(req["token"]);
                OnResponse(req.properties,new { status = success });
            }
            catch (Exception e)
            {
                OnException(e, req);
            }
        }

        private void ValidateTokenHandler(QueueRequest req)
        {

        }

        private void UpdateTokenHandler(QueueRequest req)
        {

        }

        private void UpdateToken(string token)
        {

        }
    }
}
