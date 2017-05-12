using System;
using Microsoft.Owin.Hosting;

namespace discovery_service
{
    class Program
    {
        static void Main(string[] args)
        {
            DiscoveryService.Instance.GetType();
            var options = new StartOptions()
            {
                Port = 5133,
            };
            options.Settings.Add("service_queue", DiscoveryService.QueueName);//wtf?
            //change this to non-generic version
            //add registering to service
            using (WebApp.Start<Startup>(options))
            {

                Console.WriteLine("press enter");
                Console.ReadLine();
            }
        }
    }
}
