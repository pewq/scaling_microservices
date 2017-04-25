using System;
using scaling_microservices.Auth.Tokens;
using StackExchange.Redis;

namespace test_project
{
    class Program
    {
        static void Main(string[] args)
        {
            RedisKeyValueStorage storage = new RedisKeyValueStorage();
            var a = storage.Set("pewpew", "qwe", 30);
            Console.WriteLine(storage.Get("pewpew").Result);
            Console.WriteLine(storage.Get("asd").Result);
            //var auth = new AuthService("authservice");
            //DiscoveryService.Instance.GetType();
            //var proxy = new scaling_microservices.Proxy.DiscoveryProxy(DiscoveryService.QueueName);
            //proxy.Register("main", "addr", "erwer", "type t");
            //proxy.Register("main1", "addr1", "erwer1", "type t1");
            //proxy.Ping("main", "erwer");
            
            Console.ReadLine();

            
        }
    }
}
