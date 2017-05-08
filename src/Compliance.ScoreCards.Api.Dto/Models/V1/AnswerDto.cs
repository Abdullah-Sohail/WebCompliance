using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compliance.ScoreCards.Api.Dto.Models.V1
{
    public class AnswerDto
    {
        public Guid Id { get; set; }
        public Guid MyQuestionId { get; set; }
        public string Label { get; set; }
        public int Percentage { get; set; }
        public int Order { get; set; }
    }
}
