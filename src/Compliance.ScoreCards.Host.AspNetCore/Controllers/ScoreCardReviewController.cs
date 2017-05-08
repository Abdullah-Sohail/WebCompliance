using System;
using System.Collections.Generic;
using System.Linq;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;
using Compliance.ScoreCards.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Compliance.ScoreCards.Host.AspNetCore.Controllers
{
	[Route("api/ScoreCardReview")]

	public class ScoreCardReviewController : Controller
    {
        private readonly IScoreCardService _scService;

        public ScoreCardReviewController(IScoreCardService scService)
        {
            _scService = scService;
        }


		[HttpGet("byworkitem/20131101/{id:guid}")]

		public IActionResult GetScoreCardReviewsByWorkItem_V1(Guid workItemId)
        {
            var domainResult = _scService.GetReviewsByWorkItem(workItemId).ToList();
            var ret = new ScoreCardReviews_GetByWorkItem_Result()
            {
                ScoreCardReviews = new List<ScoreCardReviewDto>()
            };

            foreach (var scr in domainResult)
                ret.ScoreCardReviews.Add(Api.DtoAdapter.V1.ScoreCardReviewAdapter.MakeDto(scr));

            return Ok(ret);
        }



		[HttpGet("byid/20131101/{id:guid}")]

		public IActionResult GetScoreCardReview(Guid id)
        {
            var ret = _scService;

            if (ret == null)
                return NotFound($"Item {id.ToString()} deosn't exist");

            throw new NotImplementedException();
        }
    }
}
