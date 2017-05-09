using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace scaling_microservices.Identity
{
    public class AppUserStore : UserStore<AppUser, AppRole, int, UserLogin, UserRole, UserClaim>
    {

        public AppUserStore(System.Data.Entity.DbContext context) : base(context) { }
        public AppUser GetByNameOwner(string name, string owner)
        {
            return this.Users.First(x => x.UserName == name && x.OwnerId == owner);
        }
    }
}
