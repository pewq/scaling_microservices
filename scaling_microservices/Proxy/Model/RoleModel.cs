using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scaling_microservices.Proxy.Model
{
    public class RoleModel
    {
        string Name { get; set; }
        string Owner { get; set; }
        List<UserModel> Participants { get; set; } = new List<UserModel>();
    }
}
