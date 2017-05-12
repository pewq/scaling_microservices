using System.Data.Entity;
using scaling_microservices.Model;



namespace scaling_microservices.Entity
{
    public class UserContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }

        public UserContext() : base() { }

        public UserContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
    }
}
