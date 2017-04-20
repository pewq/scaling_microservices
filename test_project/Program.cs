using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scaling_microservices.Rabbit;
using RabbitMQ.Client;
using discovery_service;

namespace test_project
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new DiscoveryService("disc_queue");
            var proxy = new scaling_microservices.Proxy.DiscoveryProxy("disc_queue");
            proxy.Register("main", "addr", "erwer", "type t");
            var svcs = proxy.GetServices();
            Console.WriteLine(svcs[0]);
            Console.ReadLine();

            
        }
    }
}
