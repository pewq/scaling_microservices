using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scaling_microservices.Proxy.Model;

namespace scaling_microservices.StorageStub
{
    public class RoleStorage : IStorage<RoleModel>
    {
        public static RoleStorage storage = new RoleStorage();
        public RoleStorage()
        {
            this.Add(new RoleModel()
            {
                RoleId = 1,
                Name = "default role",
                Creator = null,
                Owner = "default owner",
                Participants = new List<UserModel>() { UserStorage.storage.GetById(1),
                    UserStorage.storage.GetById(2),
                    UserStorage.storage.GetById(3)
                    }
            });
            this.Add(new RoleModel()
            {
                RoleId = 2,
                Name = "new role",
                Creator = UserStorage.storage.GetById(1),
                Participants = new List<UserModel>()
                {
                    UserStorage.storage.GetById(1),
                    UserStorage.storage.GetById(3)
                }
            });
        }
        public override RoleModel GetById(int id)
        {
            return this.Find(x => x.RoleId == id);
        }
    }
}
