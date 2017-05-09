using System;
using System.Collections.Generic;
using scaling_microservices.Rabbit;
using Newtonsoft.Json;

namespace scaling_microservices.Proxy
{
    public class DiscoveryProxy : BasicProxy
    {
        //const int responseTimeout = 100;
        public DiscoveryProxy(string route) : base(route,"")
        { }

        public DiscoveryProxy(string route, string exchange) : base(route, exchange)
        { }

        public void Ping(string name, string token)
        {
            var request = new QueueRequest() { method = "ping" };
            request["name"] = name;
            request["token"] = token;
            this.Send(request);
            endpoint.Recieve();
        }

        public void Register(string name, string address, string token, string type, string owner)
        {
            var request = new QueueRequest() { method = "register" };
            request["name"] = name;
            request["address"] = address;
            request["token"] = token;
            request["type"] = type;
            request["owner"] = owner;
            this.Send(request);
            endpoint.Recieve();
        }

        //public void Register(IService instance)
        //{
        //    var request = new QueueRequest() { method = "register" };
        //    request["name"] = instance.name;
        //    request["address"] = address;
        //    request["token"] = token;
        //    request["type"] = type;
        //    this.Send(request);
        //    endpoint.Recieve();
        //}

        public List<string> GetServices()
        {
            var request = new QueueRequest() { method = "get_services" };
            this.Send(request);
            var msg = endpoint.Recieve();
            return JsonConvert.DeserializeObject<List<string>>(msg.StringBody);
        }

        public List<object> GetData()
        {
            var request = new QueueRequest() { method = "get_all_data" };
            this.Send(request);
            var msg = endpoint.Recieve();
            var template = new { };
            var a = JsonConvert.DeserializeObject<Dictionary<string,DateTime>>(msg.StringBody);
            return new List<object>();//TODO : fix this
        }


        public bool IsAlive()
        {
            var request = new QueueRequest() { method = "is_alive" };
            this.Send(request);
            var msg = endpoint.Recieve();
            var template = new { is_alive = true };
            return JsonConvert.DeserializeAnonymousType(msg.StringBody, template).is_alive;
        }
    }
}
