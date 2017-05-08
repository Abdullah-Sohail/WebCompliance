using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Domain.Services
{
    public interface IScoreCardService
    {
        ScoreCard GetById(Guid scoreCardId);
        ICollection<ScoreCard> GetAll();

        //Results
        ScoreCardResult SaveScoreCardResult(ScoreCardResult result);

        ScoreCardReview GenerateReview(ScoreCardResult result);

        ICollection<ScoreCardResult> GetResultsByWorkItem(Guid workItemId);

        ICollection<ScoreCardReview> GetReviewsByWorkItem(Guid workItemId);

    }
}
