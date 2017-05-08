using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.ScoreCards.Api.Dto.Models.V1
{
    public class AnswerResponseDto
    {
        public Guid Id { get; set; }
        public Guid MyScoreCardResultId { get; set; }
        public Guid MyAnswerId { get; set; }
        public string Comment { get; set; }
        public DateTime UtcCreated { get; set; }
    }
}
