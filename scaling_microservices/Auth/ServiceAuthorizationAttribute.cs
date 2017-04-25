using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using scaling_microservices.Proxy;

namespace scaling_microservices.Auth
{
    public class ServiceAuthorizationAttribute : ActionFilterAttribute
    {
        AuthProxy proxy;

        public ServiceAuthorizationAttribute(string routing = "", string  exchange = "") : base()
        {
            proxy = new AuthProxy(routing, exchange);
        }

        public const string Token = "Token";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {            
            if (filterContext.Request.Headers.Contains(Token))
            {
                var tokenValue = filterContext.Request.Headers.GetValues(Token).First();

                // Validate Token
                if (!proxy.Authorize(tokenValue))
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request" };
                    filterContext.Response = responseMessage;
                }
            }
            else
            {
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.Add(Token,
                actionExecutedContext.Request.Headers.First(x => x.Key == Token).Value);
        }
    }
}
