using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Queuing.Domain.ValueTypes;

namespace Compliance.Queuing.Domain.Service
{
    public interface IPullQueueService
    {
        PullQueue GetById(Guid pullQueueId);
        ICollection<PullQueue> GetAll();
        ICollection<PullQueue> GetActive();

        void CreateQueue(PullQueue theQueue);

        ICollection<PullQueueItem> GetItems(Guid pullQueueId);
        ICollection<PullQueueItem> GetItemsHavingResults(Guid pullQueueId);
        ICollection<PullQueueItem> GetOpenItemsHavingResults(Guid pullQueueId);
        ICollection<PullQueueItem> GetOpenItems(Guid pullQueueId);
        ICollection<PullQueueItem> GetOpenItems(Guid pullQueueId, int maxCount);

        PullQueueItem GetItem(Guid pullQueueId, Guid itemId);

        void Item_AddToQueue(PullQueueItem theItem, string auditEntityType, Guid auditEntityId, string comment = "");

        void Item_View(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "");
        void Item_Assign(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "");
        void Item_CompleteScoreCard(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "");
        void Item_Escalate(Guid itemId, int newLevelOfConcern, string auditEntityType, Guid auditEntityId, string comment = "");
        void Item_Flag(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "");
        void Item_Remediate(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "");
        void Item_Close(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "");
        void Item_Reopen(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "");

        void Item_Requeue(Guid itemId, Guid newQueueId, string auditEntityType, Guid auditEntityId, string comment = "");
    }
}
