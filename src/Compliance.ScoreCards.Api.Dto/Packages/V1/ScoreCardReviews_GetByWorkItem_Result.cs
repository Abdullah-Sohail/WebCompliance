using System.Collections.Generic;
using Compliance.ScoreCards.Api.Dto.Models.V1;

namespace Compliance.ScoreCards.Api.Dto.Packages.V1
{
	public class ScoreCardReviews_GetByWorkItem_Result
	{
		public List<ScoreCardReviewDto> ScoreCardReviews { get; set; }
	}
}