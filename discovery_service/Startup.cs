﻿using Owin;
using System.Web.Http;

namespace discovery_service
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //dynamic property = appBuilder.Properties["host.Addresses"];
            //string port = property[0]["port"];
            appBuilder.UseWebApi(config);
        }
    }
}
