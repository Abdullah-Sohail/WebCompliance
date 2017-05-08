using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compliance.ScoreCards.Api.Dto.Models.V1
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public Guid MyAssertionId { get; set; }
        public Guid? MyParentAnswerId { get; set; }
        public string Query { get; set; }
        public string HelpCopy { get; set; }
        public decimal Weight { get; set; }
        public int Order { get; set; }
        public ICollection<AnswerDto> Answers { get; set; }
    }
}
