using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace scaling_microservices.Identity
{
    public class AppRoleStore : RoleStore<AppRole, int, UserRole>
    {
        public AppRoleStore(DbContext context) : base(context)
        {
        }
    }
}