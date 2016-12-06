using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace client_service
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ClientService();
            service.Start();
            Thread.Sleep(-1);
        }
    }
}
