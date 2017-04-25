using System.Collections.Generic;

namespace scaling_microservices.Proxy.Model
{
    public class GroupModel_simplified
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Creator { get; set; }
        public Dictionary<int, int> Participants { get; set; } = new Dictionary<int, int>();
    }
}
