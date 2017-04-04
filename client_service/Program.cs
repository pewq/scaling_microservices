using System;
using System.Threading;
using Microsoft.Owin.Hosting;

namespace client_service
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ClientService();
            var baseAddr = "http://localhost:5555";
            var a = WebApp.Start<Startup>(baseAddr);
            Thread.Sleep(-1);
        }
    }
}
