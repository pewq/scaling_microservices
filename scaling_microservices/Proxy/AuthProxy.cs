using System;
using Newtonsoft.Json;
using scaling_microservices.Rabbit;
using scaling_microservices.Auth.Tokens;

namespace scaling_microservices.Proxy
{
    public class AuthProxy : BasicProxy
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

        public TokenEntity BasicAuthenticate(string login, string password, string owner)
        {
            var request = new QueueRequest() { method = "authenticate" };
            request["type"] = "basic";
            request["login"] = login;
            request["password"] = password;
            request["owner"] = owner;
            Send(request);
            var msg = endpoint.Recieve();
            try
            {
                return JsonConvert.DeserializeObject<TokenEntity>(msg.StringBody);
            }
            catch(Exception)
            {
                //Delerialization exception; ill-formed object was returned
                return null;
            }
        }

        public bool ValidateToken(string token)
        {
            var request = new QueueRequest() { method = "validate" };
            request["token"] = token;
            Send(request);
            var template = new { status = true };
            var msg = endpoint.Recieve();
            return JsonConvert.DeserializeAnonymousType(msg.StringBody, template).status;
        }
    }
}
