using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using scaling_microservices.Rabbit;
using Newtonsoft.Json;

namespace scaling_microservices.Proxy
{
    public class DiscoveryProxy : IProxy
    {
        const int responseTimeout = 100;
        public DiscoveryProxy(string queue) : base(queue,"")
        {

        }

        public void Ping(string name, string token)
        {
            var request = new QueueRequest() { method = "ping" };
            request["name"] = name;
            request["token"] = token;
            this.Send(request);
            endpoint.Recieve();
        }

        public void Register(string name, string address, string token, string type)
        {
            var request = new QueueRequest() { method = "register" };
            request["name"] = name;
            request["address"] = address;
            request["token"] = token;
            request["type"] = type;
            this.Send(request);
            endpoint.Recieve();
        }

        public List<string> GetServices()
        {
            var request = new QueueRequest() { method = "get_services" };
            this.Send(request);
            var msg = endpoint.Recieve();
            return JsonConvert.DeserializeObject<List<string>>(msg.StringBody);
        }

        public bool IsAlive()
        {
            var request = new QueueRequest() { method = "is_alive" };
            this.Send(request);
            var msg = endpoint.Recieve();

        }
        /// get_services()
        /// get_all_data()
        /// ping(name,token)
        /// register(name,address,token,type)
        /// is_alive()

    }
}
