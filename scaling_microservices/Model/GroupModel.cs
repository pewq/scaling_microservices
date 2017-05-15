using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
namespace scaling_microservices.Model
{
    public class GroupModel
    {
        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public UserModel Creator { get; set; }
        public Dictionary<UserModel, HashSet<RoleModel>> Participants { get; set; } = new Dictionary<UserModel, HashSet<RoleModel>>();

        public GroupModel_simplified Simplify()
        {
            GroupModel_simplified model = new GroupModel_simplified()
            {
                GroupId = GroupId,
                Name = Name,
                Owner = Owner,
                CreatorId = Creator.UserId
            };
            model.Participants = Participants.ToDictionary(x => x.Key.UserId, x => {
                var res = new HashSet<int>();
                x.Value.Select(v => v.RoleId).ToList().ForEach(e => res.Add(e));
                return res; });
            return model;
        }
    }
}
