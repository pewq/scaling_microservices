using Microsoft.AspNet.Identity.EntityFramework;

namespace scaling_microservices.Identity
{
    public class AppRole : IdentityRole<int, UserRole>
    {
        public AppRole() : base() { }

        public string OwnerId;
    }
}
