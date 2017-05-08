using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.ScoreCards.Domain.ValueTypes
{
    public class Assertion : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyScoreCardId { get; set; }
        public string Statement { get; set; }
        public int PassThreshold { get; set; }
        public int EscalationLevelOfConcern { get; set; }
        public int Order { get; set; }

        public ICollection<Question> Questions { get; set; }

        public ScoreCard MyScoreCard { get; set; }
    }
}
