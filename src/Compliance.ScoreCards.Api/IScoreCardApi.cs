using System.Threading.Tasks;
using Compliance.ScoreCards.Api.Dto.Packages.V1;

namespace Compliance.ScoreCards.Api
{
	public interface IScoreCardApi
	{
		Task<ScoreCard_GetAll_Result> GetAllAsync();
	}
}