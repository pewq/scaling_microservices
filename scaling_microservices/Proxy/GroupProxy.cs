using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scaling_microservices.Proxy
{
    class GroupProxy : IProxy
    {
        GroupProxy(string route = "", string exchange = "") : base(route, exchange) { }

        
    }
}
