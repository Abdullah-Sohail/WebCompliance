using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Compliance.Dashboard.UI.Filters
{
    public class ValidationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                var errors = new Dictionary<string, string>();
                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];
                    if (state.Errors.Any())
                        errors.Add(key, state.Errors.FirstOrDefault().ErrorMessage);
                }

                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = "Invalid model.", errors = errors });
            }
        }
    }
}