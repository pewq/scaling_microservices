using System;
using scaling_microservices.Auth.Tokens;
using System.Linq;

namespace test_project
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpoint = new scaling_microservices.Rabbit.SubscriptionEndpoint();
            endpoint.CreateBasicProperties();
            Console.ReadLine();
        }
    }
}
