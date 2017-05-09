using Microsoft.AspNet.Identity.EntityFramework;

namespace scaling_microservices.Identity
{
    public class UserClaim : IdentityUserClaim<int>
    {
    }

    public class UserRole : IdentityUserRole<int>
    {
    }

    public class UserLogin : IdentityUserLogin<int>
    {
    }
}
