using Microsoft.Owin.Hosting;
using System.Threading;
using System;

namespace scaling_microservices
{
    class Program
    {
        static void Main(string[] args)
        {
            //DiscoveryService.Instance.Start();
            //var baseAddr = "http://localhost:8080";
            //using (WebApp.Start<Startup>(baseAddr))
            //{
            //    Console.WriteLine("press enter");
            //    Console.ReadLine();
            //}

            var e = new EventDictionary();
            e.Add("suffer", new Handler((arg) => {Console.WriteLine(arg); }));

            e.Handle("suffer", new EventArgs());
            Thread.Sleep(30);
            Console.ReadLine();
        }
    }
}
