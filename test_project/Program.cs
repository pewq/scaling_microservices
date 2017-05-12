using System;
using scaling_microservices.Auth.Tokens;
using scaling_microservices.Proxy;
using System.Linq;
using RabbitMQ.Client;
using scaling_microservices.Rabbit;
using scaling_microservices.Entity;
using scaling_microservices.Identity;
using scaling_microservices.Model;

namespace test_project
{
    using Newtonsoft.Json;
    class Program
    {
        
        static void Main(string[] args)
        {
            var str = JsonConvert.SerializeObject(null);
            var template = new { msg = "" };
            var obj = JsonConvert.DeserializeAnonymousType(str,  template);

        }
    }
}
