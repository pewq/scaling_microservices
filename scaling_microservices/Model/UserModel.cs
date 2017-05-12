using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace scaling_microservices.Model
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string OwnerId { get; set; }

        IList<RoleModel> Roles { get; set; }

        IList<GroupModel> Groups { get; set; }
    }
}
