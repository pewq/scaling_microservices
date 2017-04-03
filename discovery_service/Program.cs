using System;
using Microsoft.Owin.Hosting;

namespace discovery_service
{
    class Program
    {
        static void Main(string[] args)
        {
            DiscoveryService.Instance.GetType();
            var baseAddr = "http://localhost:5133";
            using (WebApp.Start<Startup>(baseAddr))
            {
                Console.WriteLine("press enter");
                Console.ReadLine();
            }
        }
    }
}
