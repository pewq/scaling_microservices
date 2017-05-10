using System;
using scaling_microservices.Auth.Tokens;
using scaling_microservices.Proxy;
using System.Linq;
using RabbitMQ.Client;
using scaling_microservices.Rabbit;

namespace test_project
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpoint = new EventingEndpoint();
            var queueHandle = endpoint.channel.QueueDeclarePassive(queue: auth_service.AuthService.AuthServiceQueue);
            Console.WriteLine(queueHandle.QueueName);
            Console.ReadLine();
        }
    }
}
