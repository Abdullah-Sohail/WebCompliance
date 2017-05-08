using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.Enums;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Queuing.Domain.ValueTypes
{
    public class PullQueueItem : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyPullQueueId { get; set; }
        public Guid MyWorkItemId { get; set; }
        public int MinLevelOfConcern { get; set; }
        public ICollection<PullQueueItemAction> MyActions { get; set; }
        public DateTime UtcCreated { get; set; }


        public PullQueue MyPullQueue { get; set; }

        public PullQueueItem()
        {
            Id = Guid.NewGuid();
            UtcCreated = DateTime.UtcNow;
            MyActions = new List<PullQueueItemAction>();
        }


        public PullQueueItemAction CreateAction(PullQueueItemActionEnum theAction,
            string auditEntityType, Guid auditEntityId, string comment = "")
        {
            return new PullQueueItemAction() { 
                MyPullQueueItemId = Id,
                ThisAction = theAction,
                AuditEntityType = auditEntityType,
                AuditEntityId = auditEntityId,
                Comment = comment
            };
        }
    }
}
