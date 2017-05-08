using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Domain;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Api.DtoAdapter.V1
{
    public static class ScoreCardReviewAdapter
    {
        public static ScoreCardReview MakeDomain(ScoreCardReviewDto dto)
        {
            var ret = new ScoreCardReview()
            {
                Id = dto.Id,
                MyScoreCardResultId = dto.MyScoreCardResultId,
                UtcCreated = dto.UtcCreated,
                AssertionReviews = new List<AssertionReview>()
            };

            foreach (var ar in dto.AssertionReviews)
                ret.AssertionReviews.Add(new AssertionReview()
                {
                    Id = ar.Id,
                    MyAssertionId = ar.MyAssertionId,
                    AuditEntityType = ar.AuditEntityType,
                    AuditEntityId = ar.AuditEntityId,
                    IsPassed = ar.IsPassed,
                    Passing = ar.Passing,
                    Score = ar.Score,
                    UpdateNote = ar.UpdateNote,
                    UtcLastUpdated = ar.UtcLastUpdated
                });

            return ret;
        }
        public static ScoreCardReviewDto MakeDto(ScoreCardReview domain)
        {
            var ret = new ScoreCardReviewDto()
            {
                Id = domain.Id,
                MyScoreCardResultId = domain.MyScoreCardResultId,
                UtcCreated = domain.UtcCreated,
                AssertionReviews = new List<AssertionReviewDto>()
            };

            foreach (var ar in domain.AssertionReviews)
                ret.AssertionReviews.Add(new AssertionReviewDto()
                {
                    Id = ar.Id,
                    MyAssertionId = ar.MyAssertionId,
                    MyScoreCardReviewId = domain.Id,
                    AuditEntityType = ar.AuditEntityType,
                    AuditEntityId = ar.AuditEntityId,
                    IsPassed = ar.IsPassed,
                    Passing = ar.Passing,
                    Score = ar.Score,
                    UpdateNote = ar.UpdateNote,
                    UtcLastUpdated = ar.UtcLastUpdated
                });

            return ret;
        }
    }
}
