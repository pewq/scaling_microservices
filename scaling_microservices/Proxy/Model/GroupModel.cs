using System.Collections.Generic;
using System.Linq;
namespace scaling_microservices.Proxy.Model
{
    public class GroupModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public UserModel Creator { get; set; }
        public Dictionary<UserModel, RoleModel> Participants { get; set; } = new Dictionary<UserModel, RoleModel>();

        public GroupModel_simplified Simplify()
        {
            GroupModel_simplified model = new GroupModel_simplified()
            {
                Id = GroupId,
                Name = Name,
                Owner = Owner,
                Creator = Creator.UserId
            };
            model.Participants = Participants.ToDictionary(x => x.Key.UserId, x => { return x.Value == null ? 0: x.Value.RoleId; });
            return model;
        }
    }
}
