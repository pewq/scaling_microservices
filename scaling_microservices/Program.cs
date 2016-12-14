using Microsoft.Owin.Hosting;
using System;

namespace scaling_microservices
{
    class Program
    {
        static void Main(string[] args)
        {
            DiscoveryService.Instance.Start();
            var baseAddr = "http://localhost:8080";
            using (WebApp.Start<Startup>(baseAddr))
            {
                Console.WriteLine("press enter");
                Console.ReadLine();
            }
        }
    }
}
