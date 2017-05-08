using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Compliance.Dashboard.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "QueueWork",
                url: "{team}/queue/{action}/{queue}/{level}/{id}",
                defaults: new { controller = "Queue", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "QueueHome",
                url: "{team}/queue/{queue}/{level}",
                defaults: new { controller = "Queue", action = "Index", level = 1 }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}