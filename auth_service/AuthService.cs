using System;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using scaling_microservices.Rabbit;
using scaling_microservices.Auth.Tokens;
using scaling_microservices.Identity;
using Newtonsoft.Json;

namespace auth_service
{
    public class AuthService : IService
    {
        TokenStore storage = new TokenStore();

        string dbConnectionString = "auth_database";
        //public AuthService() : base()
        //{
        //    ThisInit();
        //}

        public AuthService(string queueName, string dbNameOrConnectionString) : base(queueName)
        {
            this.dbConnectionString = dbNameOrConnectionString;
            ThisInit();
        }



        private void ThisInit()
        {
            //Init db here;
            this.Handlers.Add("register", (RequestHandleDelegate)RegistrationHandler);
            this.Handlers.Add("authenticate", (RequestHandleDelegate)AuthenticationHandler);
            this.Handlers.Add("authorize", (RequestHandleDelegate)AuthorizationHandler);
            this.Handlers.Add("broadcast_authenticate", (RequestHandleDelegate)BroadcastAuthenticationHandler);
        }

        private int handleBasicAuth(string token)
        {
            return 1;
        }

        private TokenEntity handleCredentialBasicAuth(string login, string password, string owner)
        {
            using (var ctx = new AppUserDbContext())
            { 
                AppUserManager manager = new AppUserManager(new AppUserStore(ctx));
                var user = manager.Find(login, owner);
                if(user.UserName == null)
                {
                    return null;
                }
                if (manager.CheckPassword(user, password))
                {
                    return storage.GenerateToken(user.Id);
                }
            }
            return null;
        }

        private int handleCredentialAuth(string login, string password)
        {
            return 1;
        }

        #region Handlers
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
                            //validate token 
                            //mb redirect to authrization?
                            //^ creates double call
                            //maybe skip authentication when token is provided
                            break;
                        }
                    case "basic":
                        {
                            userToken = handleCredentialBasicAuth(req["login"], req["password"], req["owner"]);
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

        private void RegistrationHandler(QueueRequest req)
        {
            try
            {
                using (var ctx = new AppUserDbContext())
                {
                    AppUserManager manager = new AppUserManager(new AppUserStore(ctx));
                    var newUser = new AppUser();
                    newUser.OwnerId = req["owner_id"];
                    newUser.UserName = req["user_name"];
                    var result = manager.Create(newUser, req["password"]);
                    OnResponse(req.properties, result.Succeeded);
                }
            }
            catch
            { }
        }


        //user_id
        //token
        //roles
        //offset
        private void BroadcastAuthenticationHandler(QueueRequest req)
        {
            try
            {
                storage.Add(req["user_id"], req["token"], JsonConvert.DeserializeObject<string[]>(req["roles"]), int.Parse(req["offset"]));
            }
            catch
            {

            }
        }
        #endregion
    }
}
