using System;
using scaling_microservices.Rabbit;

namespace auth_service
{
    class AuthService : IService
    {
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
        }

        private string handleBasicAuth(string token)
        {
            return "";
        }
        private string handleCredentialAuth(string login, string password)
        {
            return "";
        }

        private void AuthenticationHandler(QueueRequest req)
        {
            try
            {
                string authType = req["authentication"];
                string retValue = "";
                switch(authType.ToLower())
                {
                    case "basic":
                        {
                            retValue = handleBasicAuth(req["token"]);
                            break;
                        }
                    case "credential":
                        {
                            retValue = handleCredentialAuth(req["logoin"], req["password"]);
                            break;
                        }
                    default:
                        {
                            throw new Exception("Invalid authentication type");
                        }
                }
                OnResponse(req.properties, retValue);
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
                var success = authenticate(req["token"]);
                OnResponse(req.properties, success);
            }
            catch (Exception e)
            {
                OnException(e, req);
            }
        }
    }
}
