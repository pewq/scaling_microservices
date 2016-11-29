using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
namespace scaling_microservices
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddr = "http://localhost:8080";
            using (WebApp.Start<Startup>(baseAddr))
            {

                Console.WriteLine("press enter");
                Console.ReadLine();
            }

        }
    }
}
