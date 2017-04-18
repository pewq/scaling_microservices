using System.Web.Mvc;

namespace auth_service
{
    class ServiceAuthorizationAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
        }
    }
}
