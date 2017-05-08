using System.Collections.Generic;
using Compliance.ScoreCards.Api.Dto.Models.V1;

namespace Compliance.ScoreCards.Api.Dto.Packages.V1
{
	public class ScoreCardResults_GetByWorkItem_Result
	{
		public List<ScoreCardResultDto> ScoreCardResults { get; set; }
	}
}