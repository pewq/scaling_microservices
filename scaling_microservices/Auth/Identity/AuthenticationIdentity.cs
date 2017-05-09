using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace scaling_microservices.Auth.Identity
{
    public class AuthenticationIdentity : GenericIdentity
    {
        public string Password { get; set; }
        public string UserName { get; set; }
        [Key]
        public int UserId { get; set; }

        public AuthenticationIdentity(string userName, string password)
            : base(userName, "Basic")
        {
            
            Password = password;
            UserName = userName;
        }
    }
}
