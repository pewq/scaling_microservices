using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scaling_microservices.Proxy.Model;

namespace scaling_microservices.StorageStub
{
    public class UserStorage : IStorage<UserModel>
    {
        public static UserStorage storage = new UserStorage();
        public UserStorage() : base()
        {
            this.Add(new UserModel()
            {
                UserId = 1,
                Password = "first_password",
                UserName = "first!",
            });
            this.Add(new UserModel()
            {
                UserId = 2,
                Password = "second_password",
                UserName = "second!",
            });
            this.Add(new UserModel()
            {
                UserId = 3,
                Password = "third_password",
                UserName = "3!",
            });
        }

        public override UserModel GetById(int id)
        {
            return this.Find(x => x.UserId == id);
        }
    }
}
