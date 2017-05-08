using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Domain
{
    public class ScoreCardReview : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyScoreCardResultId { get; set; }
        public ICollection<AssertionReview> AssertionReviews { get; set; }
        public DateTime UtcCreated { get; set; }

        public ScoreCardResult MyScoreCardResult { get; set; }

    }
}
