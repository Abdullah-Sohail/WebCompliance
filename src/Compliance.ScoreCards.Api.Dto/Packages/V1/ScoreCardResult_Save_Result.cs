using Compliance.ScoreCards.Api.Dto.Models.V1;

namespace Compliance.ScoreCards.Api.Dto.Packages.V1
{
	public class ScoreCardResult_Save_Result
	{
		public ScoreCardResultDto MyScoreCardResultDto { get; set; }
		public ScoreCardReviewDto MyScoreCardReviewDto { get; set; }
	}
}