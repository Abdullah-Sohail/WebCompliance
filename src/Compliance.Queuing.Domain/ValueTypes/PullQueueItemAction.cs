using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.Enums;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Queuing.Domain.ValueTypes
{
    public class PullQueueItemAction : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyPullQueueItemId { get; set; }
        public PullQueueItemActionEnum ThisAction { get; set; }
        public string AuditEntityType { get; set; }
        public Guid AuditEntityId { get; set; }
        public string Comment { get; set; }
        public DateTime UtcCreated { get; set; }

        public PullQueueItem MyPullQueueItem { get; set; }

        public PullQueueItemAction()
        {
            Id = Guid.NewGuid();
            UtcCreated = DateTime.UtcNow;
        }
    }
}
