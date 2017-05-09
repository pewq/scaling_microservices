using System;
using scaling_microservices.Auth.Tokens;
using scaling_microservices.Proxy;
using System.Linq;

namespace test_project
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new AuthProxy("authservice", "");
            var status = proxy.Register("default", "default", "");
            Console.WriteLine(status);
            Console.ReadLine();
        }
    }
}
