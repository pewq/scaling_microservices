using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scaling_microservices.Proxy.Model
{
    public class GroupModel
    {
        string Name { get; set; }
        string Owner { get; set; }
        UserModel Creator { get; set; }
        Dictionary<UserModel, RoleModel> Participants { get; set; } = new Dictionary<UserModel, RoleModel>();
    }
}
