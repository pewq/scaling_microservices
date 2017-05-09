using System.Threading;
using System.Web.Http.Controllers;
using System.Security.Principal;
using scaling_microservices.Proxy;
using scaling_microservices.Auth.Identity;

namespace scaling_microservices.Auth
{
    public class ServiceAuthenticationAttribute : GenericAuthenticationAttribute
    {
        private AuthProxy proxy;

        public ServiceAuthenticationAttribute(string routing = "",
            string exchange = "", bool isActive = true) : base(isActive)
        {
            proxy = new AuthProxy(routing, exchange);
        }

        /// Protected overriden method for authorizing user
        protected override bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            //TODO : add owner
            var userToken = proxy.BasicAuthenticate(username, password, "");
            if (userToken != null)
            {
                var principal = new GenericPrincipal(Thread.CurrentPrincipal.Identity, userToken.Roles);
                var basicAuthenticationIdentity = principal.Identity as AuthenticationIdentity;
                if (basicAuthenticationIdentity != null)
                    basicAuthenticationIdentity.UserId = userToken.UserId;
                Thread.CurrentPrincipal = principal;
                actionContext.Request.Headers.Add(ServiceAuthorizationAttribute.Token, userToken.AuthToken);
                return true;
            }
            return false;
        }
    }
}
