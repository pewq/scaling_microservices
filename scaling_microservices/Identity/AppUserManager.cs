using Microsoft.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace scaling_microservices.Identity
{
    public class AppUserManager : UserManager<AppUser, int>
    {
        public AppUserManager(AppUserStore store) : base(store)
        {
        }
        public static AppUserManager Create(
        IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var manager = new AppUserManager(
                new AppUserStore(context.Get<AppUserDbContext>()));

            manager.UserValidator = new UserValidator<AppUser, int>(manager)
            {
                
            }
            // optionally configure your manager
            // ...

            return manager;
        }

        public AppUser Find(string login, string owner)
        {
            this.Store.FindByNameAsync(login);
            return new AppUser();
        }
    }
}
