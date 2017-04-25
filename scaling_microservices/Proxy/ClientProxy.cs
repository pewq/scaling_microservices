using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scaling_microservices.Proxy.Model;
using scaling_microservices.Rabbit;
using Newtonsoft.Json;

namespace scaling_microservices.Proxy
{
    public class ClientProxy : IProxy
    {
        public ClientProxy(string routing = "", string exchange = "") : base(routing, exchange) { }

        public List<UserModel> GetUsers()
        {
            var req = new QueueRequest() { method = "get_users" };
            Send(req);
            var response = endpoint.Recieve();
            var result = JsonConvert.DeserializeObject<List<UserModel>>(response.StringBody);
            return result;
        }
        public UserModel GetUser(int userId = 0, string userName = "", string userEmail = "")
        {
            var req = new QueueRequest() { method = "get_user" };
            req["user_id"] = userId.ToString();
            req["user_name"] = userName;
            req["user_email"] = userEmail;
            Send(req);
            return JsonConvert.DeserializeObject<UserModel>(endpoint.Recieve().StringBody);
        }

        public bool AddUser(UserModel user)
        {
            var req = new QueueRequest() { method = "add_user" };
            req["user"] = JsonConvert.SerializeObject(user);
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        public bool EditUser(int userId, string userName = "", string userEmail = "")
        {
            if(userName == "" && userEmail == "")
            {
                return false;
            }
            var req = new QueueRequest() { method = "edit_user" };
            req["user_id"] = userId.ToString();
            req["user_name"] = userName;
            req["user_email"] = userEmail;
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        public bool DeleteUser(int userId)
        {
            var req = new QueueRequest() { method = "delete_user" };
            req["user_id"] = userId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }
    }
}
