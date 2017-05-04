using System;
using scaling_microservices.Auth.Tokens;
using scaling_microservices.Entity;
using System.Linq;
using StackExchange.Redis;

namespace test_project
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new UserContext())
            {
                 var user = ctx.Users.First(x => x.UserId != 0);
            }
            Console.ReadLine();
        }
    }
}
