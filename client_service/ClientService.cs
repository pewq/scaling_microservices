using System;
using System.Linq;
using Newtonsoft.Json;
using scaling_microservices.Model;
using scaling_microservices.StorageStub;
using scaling_microservices.Rabbit;
using scaling_microservices.Entity;

namespace client_service
{
    public class ClientService : IService
    {
        public const string QueueName = "ClientCommandQueue";

        //TODO: implement locking mechanism for whole service
        //when no discovery service is found
        bool discoveryFoundFlag;
        string discoveryServicePath;

        public ClientService() : base(QueueName)
        {
            ThisInit();
        }

        private void ThisInit()
        {
            Handlers.Add("register_discovery", (RequestHandleDelegate)discoveryRegisterHandler);
            Handlers.Add("get_users", (RequestHandleDelegate)GetUsersHandler);
            Handlers.Add("get_user", (RequestHandleDelegate)SearchUserHandler);
            Handlers.Add("add_user", (RequestHandleDelegate)AddUserHandler);
            Handlers.Add("edit_user", (RequestHandleDelegate)EditUserHandler);
            Handlers.Add("delete_user", (RequestHandleDelegate)DeleteUserHandler);
        }
        
        private bool CheckDiscoveryHealth()
        {
            var ep = new SubscriptionEndpoint();

            ep.SendTo(new QueueRequest() { method = "is_alive" }, discoveryServicePath);
            var response = ep.Recieve(1 * 1000);
            return !ReferenceEquals(null, response);
        }

        #region Handlers
        private void discoveryRegisterHandler(QueueRequest req)
        {
            try
            {
                discoveryServicePath = req["address"];
                Console.WriteLine(discoveryServicePath);
            }
            catch(Exception e)
            {
                OnException(e, req);
            }
        }

        private void GetUsersHandler(QueueRequest req)
        {
            var context = new UserContext();
            OnResponse(req.properties, context.Users.ToList());
        }

        private void SearchUserHandler(QueueRequest req)
        {

            string parameter = req.Contains("user_id");
            if(parameter != null && parameter != "0")
            {
                using (var ctx = new UserContext())
                {
                    var user = ctx.Users.SingleOrDefault(x => x.UserId == int.Parse(parameter));
                    OnResponse(req.properties, user);
                    return;
                }
            }
            parameter = req.Contains("user_name");
            if(!String.IsNullOrEmpty(parameter))
            {
                using (var ctx = new UserContext())
                {
                    var users = ctx.Users.Where(x => x.UserName == parameter);
                    OnResponse(req.properties, users);
                    return;
                }
            }
            OnResponse(req.properties, null);
        }

        private void AddUserHandler(QueueRequest req)
        {
            int resultId;
            UserModel user = null;
            try
            {
                user = JsonConvert.DeserializeObject<UserModel>(req["user"]);
            }
            catch (Exception e)
            {
                OnException(e, req);
            }
            if(user != null)
            {
                using (var ctx = new UserContext())
                {
                    var existingUser = ctx.Users.FirstOrDefault(x => x.UserId == user.UserId);
                    if (existingUser != null)
                    {
                        resultId = -1;
                    }
                    else
                    {
                        ctx.Users.Add(user);
                        resultId = user.UserId;
                    }
                }                
            }
            else
            {
                resultId = 0;
            }
            OnResponse(req.properties, resultId);
        }

        private void EditUserHandler(QueueRequest req)
        {
            using (var ctx = new UserContext())
            {
                var user = ctx.Users.FirstOrDefault(x => x.UserId == int.Parse(req["user_id"]));
                if (user == null)
                {
                    OnResponse(req.properties, false);
                    return;
                }
                if (req.arguments.ContainsKey("user_name") && req["user_name"] != "")
                {
                    user.UserName = req["user_name"];
                }
                if (req.arguments.ContainsKey("user_email") && req["user_email"] != "")
                {
                    user.UserName = req["user_email"];
                }
                ctx.SaveChanges();
                OnResponse(req.properties, true);
            }
        }

        private void DeleteUserHandler(QueueRequest req)
        {
            using (var ctx = new UserContext())
            {
                var user = ctx.Users.First(x => x.UserId == int.Parse(req["user_id"]));
                if (user != null) {
                    ctx.Users.Remove(user);
                    OnResponse(req.properties, true);
                }
                else
                {
                    OnResponse(req.properties, false);
                }
            }
        }
        #endregion

    }
}
