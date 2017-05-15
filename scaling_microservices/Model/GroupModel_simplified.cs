using System.Collections.Generic;

namespace scaling_microservices.Model
{
    public class GroupModel_simplified
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int CreatorId { get; set; }
        public Dictionary<int,HashSet<int>> Participants { get; set; } = new Dictionary<int,HashSet<int>>();
    }
}
