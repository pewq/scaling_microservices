using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using scaling_microservices.Proxy;

namespace scaling_microservices.Auth
{
    public class ServiceAuthorizationAttribute : AuthorizeAttribute 
    {
        AuthProxy proxy;
        public ServiceAuthorizationAttribute(string routing = "", string exchange = "")
        {
            proxy = new AuthProxy(routing, exchange);
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var authHeader = actionContext.Request.Headers.Authorization;
            var success = proxy.Authorize(authHeader.Parameter);
            if(success)
            {
                return TaskHelpers.Completed();
            }
            else
            {
                return TaskHelpers.FromError(new System.Exception("nope"));
            }
        }
    }
}
