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
        public const string AuthServiceQueue = "auth_service";
        const string BroadcastExchange = "auth_fanout_exchange";

        TokenStore storage = new TokenStore();

        EventingEndpoint broadcastEndpoint;

        string dbConnectionString = "auth_database";

        public AuthService(string queueName, string dbNameOrConnectionString) : base(queueName)
        {
            this.dbConnectionString = dbNameOrConnectionString;
            ThisInit();
        }

        public AuthService(string dbNameOrConnectionString) : base(AuthServiceQueue, true)
        {
            this.dbConnectionString = dbNameOrConnectionString;
            ThisInit();
        }

        private void ThisInit()
        {
            //init exchange for broadcast
            broadcastEndpoint = new EventingEndpoint();
            //"fanout" is const string for type of exchange.
            broadcastEndpoint.ExchangeExistsOrDeclare(BroadcastExchange, "fanout");
            //bind endpoint.Queue to echange
            broadcastEndpoint.Bind(BroadcastExchange, "");
            broadcastEndpoint.OnRecieved += (obj, message) => {
                if(message.Properties.ReplyTo == endpoint.InQueue || message.Properties.ReplyTo == broadcastEndpoint.InQueue)
                {
                    return;
                }
                if(message.Encoding == QueueRequest.classname)
                {
                    QueueRequest req = new QueueRequest(message.body, message.Properties);
                    this.BroadcastAuthenticationHandler(req);
                }
            };

            this.Handlers.Add("register", (RequestHandleDelegate)RegistrationHandler);
            this.Handlers.Add("authenticate", (RequestHandleDelegate)AuthenticationHandler);
            this.Handlers.Add("authorize", (RequestHandleDelegate)AuthorizationHandler);
        }

        private int handleBasicAuth(string token)
        {
            return 1;
        }
        #region Auth Handlers
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
                    var roles = manager.GetRoles(user.Id);
                    return storage.GenerateToken(user.Id, roles.ToArray());
                }
            }
            return null;
        }

        private int handleCredentialAuth(string login, string password)
        {
            return 1;
        }
        #endregion
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
                OnResponseWBCast(req.properties, userToken);
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
                    var proxy = new scaling_microservices.Proxy.ClientProxy("ClientCommandQueue", "");
                    
                    if (result.Succeeded)
                    {
                        var model = new scaling_microservices.Model.UserModel()
                        {
                            Email = "",
                            OwnerId = newUser.OwnerId,
                            UserName = newUser.UserName,
                            UserId = manager.Find(newUser.UserName, newUser.OwnerId).Id
                        };
                        int userId = proxy.AddUser(model);
                        if(userId == model.UserId)
                        {
                            OnResponse(req.properties, result.Succeeded);
                        }
                        else
                        {
                            ctx.Users.Remove(newUser);
                            OnException(new OperationCanceledException("client service was unable to register user"), req);

                        }
                    }
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
                var token = JsonConvert.DeserializeObject<TokenEntity>(req["user_token"]);
                storage.Add(token);
            }
            catch
            {
            }
        }
        #endregion

        protected void OnResponseWBCast(RabbitMQ.Client.IBasicProperties props, object obj)
        {
            QueueRequest req = new QueueRequest();
            req["user_token"] = JsonConvert.SerializeObject(obj as TokenEntity);
            endpoint.SendTo(req, "", BroadcastExchange);
            base.OnResponse(props, obj);
        }
    }
}
