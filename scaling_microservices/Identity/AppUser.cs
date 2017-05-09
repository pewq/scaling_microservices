using Microsoft.AspNet.Identity.EntityFramework;

namespace scaling_microservices.Identity
{
    public class AppUser : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        public string OwnerId { get; set; }
    }
}
