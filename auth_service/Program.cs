using System;
using System.Threading;
using Microsoft.Owin.Hosting;

namespace auth_service
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new AuthService("authservice");
            Thread.Sleep(-1);
        }
    }
}
