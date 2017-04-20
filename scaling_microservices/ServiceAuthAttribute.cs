using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using scaling_microservices.Proxy;

namespace scaling_microservices
{
    class ServiceAuthAttribute : AuthorizeAttribute 
    {
        AuthProxy proxy;
        public ServiceAuthAttribute(string routing = "", string exchange = "")
        {
            proxy = new AuthProxy(routing, exchange);
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}
