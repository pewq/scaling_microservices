using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace scaling_microservices
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:9090");

            config.Routes.MapHttpRoute(
                "Default", "{controller}/id", new { id = RouteParameter.Optional }
                );
        }
    }
}
