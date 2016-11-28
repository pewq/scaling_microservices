using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scaling_microservices
{
    class Program
    {
        static void Main(string[] args)
        {
            var registry = new ServiceRegistry(10);
            registry.Add("this !@#%");
            Console.WriteLine(registry.Get().Count);
            var timer = new System.Timers.Timer(20 * 1000) { Enabled = true, AutoReset = true };
            timer.Elapsed += (sender, e) =>
               {
                   Console.WriteLine(registry.Get().Count);
               };
            Console.ReadLine();
        }
    }
}
