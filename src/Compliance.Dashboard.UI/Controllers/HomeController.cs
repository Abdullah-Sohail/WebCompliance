using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Compliance.ScoreCards.Api.Dto.Models.V1;

namespace Compliance.Dashboard.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
	        try
	        {
            ViewBag.ActiveMenu = "Home";

            //var scoreCards = CacheHelper.GetScoreCards(System.Web.HttpContext.Current);

		        ViewBag.ScoreCard = Enumerable.Empty<ScoreCardDto>();


	        }
	        catch (Exception ex)
	        {
		        
	        }

            return View();
        }
    }
}
