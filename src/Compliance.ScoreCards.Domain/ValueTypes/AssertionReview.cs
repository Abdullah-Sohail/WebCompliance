using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compliance.ScoreCards.Domain.ValueTypes
{
    public class AssertionReview
    {
        public Guid Id { get; set; }
        public Guid MyAssertionId { get; set; }
        public Guid MyScoreCardReviewId { get; set; }
        public decimal Score { get; set; }
        public decimal Passing { get; set; }
        public bool IsPassed { get; set; }
        public string UpdateNote { get; set; }
        public DateTime UtcLastUpdated { get; set; }
        public string AuditEntityType { get; set; }
        public Guid AuditEntityId { get; set; }

        public Assertion MyAssertion { get; set; }
        public ScoreCardReview MyScoreCardReview { get; set; }
    }
}
