using System.Threading;
using System.Web.Http.Controllers;
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
            var userToken = proxy.BasicAuthenticate(username, password);
            if (userToken != null)
            {
                var basicAuthenticationIdentity = Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                if (basicAuthenticationIdentity != null)
                    basicAuthenticationIdentity.UserId = userToken.UserId;
                actionContext.Request.Headers.Add(ServiceAuthorizationAttribute.Token, userToken.AuthToken);
                //actionContext.Request.Headers.Add("Expires", userToken.ExpiresOn.ToFileTime().ToString());
                //actionContext.Response.Headers.Add(ServiceAuthorizationAttribute.Token, userToken.AuthToken);
                //actionContext.Response.Headers.Add("Expires", userToken.ExpiresOn.ToFileTime().ToString());
                return true;
            }
            return false;
        }
    }
}
