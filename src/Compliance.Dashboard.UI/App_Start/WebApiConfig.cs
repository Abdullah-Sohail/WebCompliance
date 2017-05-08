using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Compliance.Dashboard.UI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{command}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

	       // config.MapHttpAttributeRoutes();

		}
	}
}