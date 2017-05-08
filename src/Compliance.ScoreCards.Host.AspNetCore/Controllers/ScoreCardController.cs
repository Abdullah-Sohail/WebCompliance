using System.Collections.Generic;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;
using Compliance.ScoreCards.Api.DtoAdapter.V1;
using Compliance.ScoreCards.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Compliance.ScoreCards.Host.AspNetCore.Controllers
{
	[Route("api/ScoreCard")]
	public class ScoreCardController : Controller
    {
        private readonly IScoreCardService _scService;

        public ScoreCardController(IScoreCardService scService)
        {
            _scService = scService;
        }



		[HttpGet("all")]
		[HttpGet("all/20131101")]

		public IActionResult GetAllScoreCards()
        {
            var retContent = new ScoreCard_GetAll_Result(){
                ScoreCardDtos = new List<ScoreCardDto>()
            };
            
            foreach(var sc in _scService.GetAll())
                retContent.ScoreCardDtos.Add(ScoreCardAdapter.MakeDto(sc));

	        return Ok(retContent);
        }
    }
}
