using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.ScoreCards.Api.Dto.Models.V1
{
    public class ScoreCardReviewDto
    {
        public Guid Id { get; set; }
        public Guid MyScoreCardResultId { get; set; }
        public ICollection<AssertionReviewDto> AssertionReviews { get; set; }
        public DateTime UtcCreated { get; set; }
    }
}
