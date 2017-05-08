using System;
using System.Collections.Generic;
using System.Linq;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;
using Compliance.ScoreCards.Api.DtoAdapter.V1;
using Compliance.ScoreCards.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Compliance.ScoreCards.Host.AspNetCore.Controllers
{
	[Route("api/ScoreCardResult")]
    public class ScoreCardResultController : Controller
    {
        private IScoreCardService _scService;

        public ScoreCardResultController(IScoreCardService scService)
        {
            _scService = scService;
        }

		[HttpGet("byworkitem/20131101/{id}")]
		public IActionResult GetScoreCardResultsByWorkItem(Guid workItemId)
        {
            var domainResult = _scService.GetResultsByWorkItem(workItemId).ToList();
            var ret = new ScoreCardResults_GetByWorkItem_Result(){
                ScoreCardResults = new List<ScoreCardResultDto>()
            };

            foreach (var scr in domainResult)
                ret.ScoreCardResults.Add(Api.DtoAdapter.V1.ScoreCardResultAdapter.MakeDto(scr));

	        return Ok(ret);
        }
		
		[HttpGet("byid/20131101/{id}")]
        public IActionResult GetScoreCardResult(Guid id)
        {
            var ret = _scService;

            if (ret == null)
                return NotFound($"Item {id} deosn't exist");

            throw new NotImplementedException();
        }

		[HttpPost("byid/20131101")]
		public  IActionResult SaveScoreCardResult([FromBody] ScoreCardResult_Save_Command cmd)
        {
            var retContent = new ScoreCardResult_Save_Result();

            try
            {
                var newResult = ScoreCardResultAdapter.MakeDomain(cmd.MyScoreCardResult);

                newResult = _scService.SaveScoreCardResult(newResult);

                var theReview = _scService.GenerateReview(newResult);

                //Build Dto response
                retContent.MyScoreCardResultDto = ScoreCardResultAdapter.MakeDto(newResult);

                retContent.MyScoreCardReviewDto = ScoreCardReviewAdapter.MakeDto(theReview);

	            var ret = Created(new Uri("/api/scorecardresult/byid/20131101/{id}"), retContent);

				//TODO: Add Location Header to get results in the future to be RESTful
				//ret.Headers.Location 

				return ret;
            }
            catch (Exception ex)
            {
                var msg = $"Err: {ex.Message}";

                if (ex.InnerException != null)
                    msg += $" Inner: {ex.InnerException.Message}";
                if (ex.InnerException?.InnerException != null)
                    msg += $" DB: {ex.InnerException.InnerException.Message}";

                return BadRequest(msg);
            }

        }
    }
}
