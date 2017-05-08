using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Domain
{
    public class ScoreCard : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ICollection<Assertion> Assertions { get; set; }

        public int Version { get; set; }
        public Guid? MyLastVersionId { get; set; }
        public DateTime UtcCreated { get; set; }

        public ScoreCard MyLastVersion { get; set; }
    }
}
