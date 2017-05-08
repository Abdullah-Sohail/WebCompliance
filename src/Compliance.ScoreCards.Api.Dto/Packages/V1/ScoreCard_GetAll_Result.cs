using System.Collections.Generic;
using Compliance.ScoreCards.Api.Dto.Models.V1;

namespace Compliance.ScoreCards.Api.Dto.Packages.V1
{
	public class ScoreCard_GetAll_Result
	{
		public ScoreCard_GetAll_Result()
		{
			ScoreCardDtos = new List<ScoreCardDto>();
		}
		public ICollection<ScoreCardDto> ScoreCardDtos { get; set; }
	}
}