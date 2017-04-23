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
//            var service = new DiscoveryService(DIs);
            var proxy = new scaling_microservices.Proxy.DiscoveryProxy(DiscoveryService.QueueName);
            proxy.Register("main", "addr", "erwer", "type t");
            proxy.Register("main1", "addr1", "erwer1", "type t1");
            proxy.Ping("main", "erwer");
            
            Console.ReadLine();

            
        }
    }
}
