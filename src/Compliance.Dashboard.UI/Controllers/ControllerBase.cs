using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Compliance.Dashboard.Domain;

namespace Compliance.Dashboard.UI.Controllers
{
    public class ControllerBase : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Profile = CacheHelper.GetProfileByUserId(User.Identity.GetUserId(), DependencyResolver.Current);
            }

        }
    }
}
