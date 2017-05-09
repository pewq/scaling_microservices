using scaling_microservices.Auth.Identity;

namespace scaling_microservices.Proxy.Model
{
    public class UserModel : AuthenticationIdentity
    {
        public UserModel() : base("", "") { }

        public UserModel(string userName, string password):base(userName, password) { }
    }
}
