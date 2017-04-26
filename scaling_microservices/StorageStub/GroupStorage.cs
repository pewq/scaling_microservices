using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scaling_microservices.Proxy.Model;

namespace scaling_microservices.StorageStub
{
    public class GroupStorage : IStorage<GroupModel>
    {
        static GroupStorage storage = new GroupStorage();

        public GroupStorage()
        {
            this.Add(new GroupModel()
            {
                GroupId = 1,
                Name = "first group",
                Owner = "default",
                Creator = UserStorage.storage.GetById(1),
                Participants = new Dictionary<UserModel, List<RoleModel>>
                {
                    { UserStorage.storage.GetById(1), new List<RoleModel>() {
                        RoleStorage.storage.GetById(1)
                    } },
                    { UserStorage.storage.GetById(3), new List<RoleModel>() {
                        RoleStorage.storage.GetById(1)
                    } }
                }
            });


        }

        public override GroupModel GetById(int id)
        {
            return this.Find(x => x.GroupId == id);
        }
    }
}
