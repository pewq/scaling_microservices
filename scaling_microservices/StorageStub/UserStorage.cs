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
                Login = "first_user",
                Password = "first_password",
                Email = "first@example.com",
                Name = "first!",
            });
            this.Add(new UserModel()
            {
                UserId = 2,
                Login = "second_user",
                Password = "second_password",
                Email = "second@example.com",
                Name = "second!",
            });
            this.Add(new UserModel()
            {
                UserId = 3,
                Login = "third_user",
                Password = "third_password",
                Email = "third@example.com",
                Name = "3!",
            });
        }

        public override UserModel GetById(int id)
        {
            return this.Find(x => x.UserId == id);
        }
    }
}
