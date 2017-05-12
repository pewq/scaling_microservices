using System.Collections.Generic;

namespace scaling_microservices.Model
{
    public class RoleModel_simplified
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Creator { get; set; }
        public List<int> Participants { get; set; } = new List<int>();
    }
}
