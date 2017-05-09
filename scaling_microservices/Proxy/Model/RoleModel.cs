using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;

namespace scaling_microservices.Proxy.Model
{
    public class RoleModel
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public UserModel Creator { get; set; }
        public List<UserModel> Participants { get; set; } = new List<UserModel>();

        public RoleModel_simplified Simplify()
        {
            var model = new RoleModel_simplified()
            {
                RoleId = RoleId,
                Name = Name,
                Owner = Owner,
                Creator = Creator.UserId,
                Participants = Participants.Select(x => x.UserId).ToList()
            };
            return model;
        }
    }
}
