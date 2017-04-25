using System.Collections.Generic;

namespace scaling_microservices.Proxy.Model
{
    public class RoleModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public List<UserModel> Participants { get; set; } = new List<UserModel>();
    }
}
