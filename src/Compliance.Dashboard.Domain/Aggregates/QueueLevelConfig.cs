using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Dashboard.Domain
{
    public class QueueLevelConfig : ICanGenericRepo
    {
        public Guid Id { get; set; }

        public Guid QueueId { get; set; }

        public int QueueLevel { get; set; }

        public string MenuName { get; set; }

        public int SupervisorLevel { get; set; }

        public int ViewTimeout { get; set; }

        public int AssignTimeout { get; set; }

        public int ExtendMinutes { get; set; }

        public string ActionName { get; set; }

        public DateTime UtcCreated { get; set; }
    }
}
