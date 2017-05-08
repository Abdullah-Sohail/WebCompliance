using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Dashboard.Domain.ValueType
{
    public class QueueAssignment: ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyTeamId { get; set; }
        public Guid MyQueueId { get; set; }
        public DateTime UtcCreated { get; set; }

        public Team MyTeam { get; set; }

    }
}
