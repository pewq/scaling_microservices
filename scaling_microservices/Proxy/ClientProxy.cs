using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace scaling_microservices.Proxy
{
    public class ClientProxy : IProxy
    {
        public ClientProxy(string routing = "", string exchange = "") : base(routing, exchange) { }

        public List<UserModel>
    }
}
