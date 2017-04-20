using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using scaling_microservices.Rabbit;

namespace scaling_microservices.Proxy
{
    public class AuthProxy : IProxy
    {
        public AuthProxy(string _route = "", string _exchange = "") : base(_route, _exchange)
        { }

        public bool Authorize(string token)
        {
            var request = new QueueRequest() { method = "authorize" };
            request["token"] = token;
            Send(request);
            var template = new { status = true };
            var msg = endpoint.Recieve();
            return JsonConvert.DeserializeAnonymousType(msg.StringBody, template).status;
        }

        public bool Authorize(string login, string password)
        {
            var request = new QueueRequest() { method = "authorize" };
            request["login"] = login;
            request["password"] = password;
            Send(request);
            var template = new { status = true };
            var msg = endpoint.Recieve();
            return JsonConvert.DeserializeAnonymousType(msg.StringBody, template).status;
        }
    }
}
