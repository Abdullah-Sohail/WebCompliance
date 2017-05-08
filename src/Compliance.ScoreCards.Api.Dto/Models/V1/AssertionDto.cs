using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compliance.ScoreCards.Api.Dto.Models.V1
{
    public class AssertionDto
    {
        public Guid Id { get; set; }
        public Guid MyScoreCardId { get; set; }
        public string Statement { get; set; }
        public int PassThreshold { get; set; }
        public int EscalationLevelOfConcern { get; set; }
        public int Order { get; set; }
        public ICollection<QuestionDto> Questions { get; set; }
    }
}
