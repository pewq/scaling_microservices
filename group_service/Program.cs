using System;
using Microsoft.Owin.Hosting;

namespace group_service
{
    class Program
    {
        static void Main(string[] args)
        {
            GroupService service = new GroupService();
            var baseAddr = "http://localhost:5133";
            using (WebApp.Start<Startup>(baseAddr))
            {
                Console.WriteLine("press enter");
                Console.ReadLine();
            }
        }
    }
}
