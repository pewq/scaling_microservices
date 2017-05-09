using System;
using System.Linq;
using Newtonsoft.Json;
using scaling_microservices.Proxy.Model;
using scaling_microservices.StorageStub;
using scaling_microservices.Rabbit;

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
            var users = UserStorage.storage.ToList();
            OnResponse(req.properties, users);
        }

        private void SearchUserHandler(QueueRequest req)
        {
            var users = UserStorage.storage.ToList();
            if (req["user_id"] != "0")
            {
                users = users.Where(x => x.UserId.ToString() == req["user_id"]).ToList();
            }
            if(req["user_name"] != "")
            {
                users = users.Where(x => x.Name == req["user_name"]).ToList();
            }
            OnResponse(req.properties, users.First());
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
                var existingUser = UserStorage.storage.GetById(user.UserId);
                if(existingUser != null)
                {
                    resultId = -1;
                }
                else
                {
                    if(user.UserId != 0)
                    {
                        //Generate UserId
                    }
                    resultId = user.UserId;
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
            var user = UserStorage.storage.GetById(int.Parse(req["user_id"]));
            if(user != null)
            {
                OnResponse(req.properties, false);
                return;
            }
            if(req["user_name"] != "")
            {
                user.UserName = req["user_name"];
            }
            //idk, whether user was set

            OnResponse(req.properties, true);
        }

        private void DeleteUserHandler(QueueRequest req)
        {
            var user = UserStorage.storage.GetById(int.Parse(req["user_id"]));
            if(user == null)
            {
                OnResponse(req.properties, false);
                return;
            }
            UserStorage.storage.Remove(user);
            OnResponse(req.properties, true);
        }
        #endregion

    }
}
