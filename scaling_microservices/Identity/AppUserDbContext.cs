using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity.EntityFramework;

namespace scaling_microservices.Identity
{
    public class AppUserDbContext : IdentityDbContext<AppUser, AppRole, int, UserLogin, UserRole, UserClaim>
    {
        public AppUserDbContext() : base() { }

        public AppUserDbContext(string nameOrConnectionString) :
            base(nameOrConnectionString)
        { }

        protected override DbEntityValidationResult ValidateEntity(
            DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            if (entityEntry != null && entityEntry.State == EntityState.Added)
            {
                var errors = new List<DbValidationError>();
                var user = entityEntry.Entity as  AppUser;

                if (user != null)
                {
                    if (this.Users.Any(u => string.Equals(u.UserName, user.UserName)
                      && u.OwnerId == user.OwnerId))
                    {
                        errors.Add(new DbValidationError("User",
                          string.Format("Username {0} is already taken for AppId {1}",
                            user.UserName, user.OwnerId)));
                    }

                    if (this.RequireUniqueEmail
                      && this.Users.Any(u => string.Equals(u.Email, user.Email)
                      && u.OwnerId == user.OwnerId))
                    {
                        errors.Add(new DbValidationError("User",
                          string.Format("Email Address {0} is already taken for AppId {1}",
                            user.UserName, user.OwnerId)));
                    }
                }
                else
                {
                    var role = entityEntry.Entity as IdentityRole;

                    if (role != null && this.Roles.Any(r => string.Equals(r.Name, role.Name)))
                    {
                        errors.Add(new DbValidationError("Role",
                          string.Format("Role {0} already exists", role.Name)));
                    }
                }
                if (errors.Any())
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }

            return new DbEntityValidationResult(entityEntry, new List<DbValidationError>());
        }
    }
}
