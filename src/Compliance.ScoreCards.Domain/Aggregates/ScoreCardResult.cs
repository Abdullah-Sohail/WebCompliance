using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Domain
{
    public class ScoreCardResult : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyScoreCardId { get; set; }
        public string WorkItemType { get; set; }
        public Guid WorkItemId { get; set; }
        public string AuditEntityType { get; set; }
        public Guid AuditEntityId { get; set; }
        public DateTime UtcCreated { get; set; }

        public ICollection<AnswerResponse> Answers { get; set; }

        public ScoreCard MyScoreCard { get; set; }

    }
}
