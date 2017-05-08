using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.ScoreCards.Api.Dto.Models.V1
{
    public class ScoreCardResultDto
    {
        public Guid Id { get; set; }
        public Guid MyScoreCardId { get; set; }
        public string WorkItemType { get; set; }
        public Guid WorkItemId { get; set; }
        public string AuditEntityType { get; set; }
        public Guid AuditEntityId { get; set; }
        public ICollection<AnswerResponseDto> Answers { get; set; }
        public DateTime UtcCreated { get; set; }
    }
}
