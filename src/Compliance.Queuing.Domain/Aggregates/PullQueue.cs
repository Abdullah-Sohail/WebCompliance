using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.Enums;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Queuing.Domain.ValueTypes;

namespace Compliance.Queuing.Domain
{
    public class PullQueue : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public string QueueName { get; set; }
        public WorkItemTypeEnum WorkItemType { get; set; }
        public ICollection<PullQueueItem> MyPullQueueItems { get; set; }
        public bool Active { get; set; }
        public DateTime UtcCreated { get; set; }
    }
}
