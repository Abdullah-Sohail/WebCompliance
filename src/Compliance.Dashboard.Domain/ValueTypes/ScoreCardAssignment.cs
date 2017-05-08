using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Dashboard.Domain.ValueTypes
{
    public class ScoreCardAssignment : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyTeamId { get; set; }
        public Guid MyScoreCardId { get; set; }
        public int MinLevelOfConcern { get; set; }
        public bool Active { get; set; }
        public DateTime UtcCreated { get; set; }

        public Team MyTeam { get; set; }
    }
}
