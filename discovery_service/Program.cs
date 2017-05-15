using System;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Hosting;

namespace discovery_service
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new DiscoveryService(DiscoveryService.QueueName);
            service.LogFunction += (str) =>
            {
                Console.WriteLine(str);
            };
            var options = new StartOptions()
            {
                Port = new Random().Next(5000, 5000 + 300)
            };
            if (args.Length > 0)
            {
                using (WebApp.Start(options, (appBuilder) =>
                {
                    HttpConfiguration config = new HttpConfiguration();
                    config.Routes.MapHttpRoute(
                        name: "DefaultApi",
                        routeTemplate: "api/{controller}/{action}/{id}",
                        defaults: new { id = RouteParameter.Optional }
                    );

                    dynamic property = appBuilder.Properties["host.Addresses"];
                    string port = property[0]["port"];
                    Console.WriteLine(port);
                    appBuilder.UseWebApi(config);
                }))
                {
                    DiscoveryController.HasController = true;
                    Console.WriteLine("press enter");
                    Console.ReadLine();
                    DiscoveryController.HasController = false;
                }
            }
            else
            {
                Console.ReadLine();
            }
        }
    }
}
