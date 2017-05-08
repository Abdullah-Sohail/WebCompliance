using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;
using Compliance.ScoreCards.Domain.Services;

namespace Compliance.ScoreCards.Api.MvcService.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IScoreCardService service)
        {
           
        }
        public ActionResult Index()
        {
            //var scr = new ScoreCardResultDto()
            //{
            //    WorkItemType = "Recording",
            //    WorkItemId = Guid.NewGuid(),
            //    MyScoreCardId = Guid.NewGuid(),
            //    AuditEntityType = "User",
            //    AuditEntityId = Guid.NewGuid(),
            //    UtcCreated = DateTime.UtcNow
            //};

            //var ans = new List<AnswerResponseDto>();

            //for (var i = 0; i < 10; i++)
            //{
            //    ans.Add(new AnswerResponseDto()
            //    {
            //        Comment = "Response " + i.ToString(),
            //        MyAnswerId = Guid.NewGuid()
            //    });
            //}

            //var result = new ScoreCardResultApi("", "").SaveAsync(scr, ans).Result;

            
            return View();
        }
    }
}
