using System;
using System.Collections.Generic;
using System.Linq;
using Compliance.Common.Enums;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Queuing.Domain;
using Compliance.Queuing.Domain.Service;
using Compliance.Queuing.Domain.ValueTypes;

namespace Compliance.Queuing.Implementation.Ef.Services
{
    public class PullQueueService : IPullQueueService
    {
        private IGenericRepo<PullQueue, QueuingContext> _pullQueueRepo;
        private IGenericRepo<PullQueueItem, QueuingContext> _itemRepo;
        private IGenericRepo<PullQueueItemAction, QueuingContext> _itemActionRepo;

        private string[] _pullQueueItemIncludes = new string[] { "MyActions" };

        public PullQueueService(IGenericRepo<PullQueue, QueuingContext> pullQueueRepo,
            IGenericRepo<PullQueueItem, QueuingContext> itemRepo, IGenericRepo<PullQueueItemAction, QueuingContext> itemActionRepo)
        {
            _pullQueueRepo = pullQueueRepo;
            _itemRepo = itemRepo;
            _itemActionRepo = itemActionRepo;
        }

        public PullQueue GetById(Guid pullQueueId)
        {
            return _pullQueueRepo.GetById(pullQueueId);
        }

        public ICollection<PullQueue> GetAll()
        {
            return _pullQueueRepo.GetAll().ToList();
        }

        public ICollection<PullQueue> GetActive()
        {
            return _pullQueueRepo
                .Query(q => q.Active == true)
                .ToList();
        }

        public void CreateQueue(PullQueue theQueue)
        {
            //TODO: VALIDATE PULL QUEUE
            _pullQueueRepo.Add(theQueue);
            _pullQueueRepo.Save();
        }

        public ICollection<PullQueueItem> GetItems(Guid pullQueueId)
        {
            return _itemRepo
                .Query(i => i.MyPullQueueId == pullQueueId, _pullQueueItemIncludes)
                .ToList();
        }

        public ICollection<PullQueueItem> GetOpenItems(Guid pullQueueId)
        {
            var allItems = _itemRepo
                .Query(i => i.MyPullQueueId == pullQueueId, _pullQueueItemIncludes);

            //Reopen 15 minute views
            foreach (var i in allItems.Where(
                a => a.MyActions
                    .OrderByDescending(c => c.UtcCreated)
                    .First()
                    .ThisAction == PullQueueItemActionEnum.Viewed)
                .ToList())
                if (i.MyActions.OrderByDescending(d => d.UtcCreated).First().UtcCreated < DateTime.UtcNow.AddMinutes(-15))
                    i.MyActions.Add(new PullQueueItemAction()
                    {
                        AuditEntityId = Guid.NewGuid(),
                        AuditEntityType = "System",
                        Comment = "Timeout",
                        Id = Guid.NewGuid(),
                        MyPullQueueItemId = i.Id,
                        ThisAction = PullQueueItemActionEnum.Opened,
                        UtcCreated = DateTime.UtcNow
                    });

            //Reopen 60 minute assignments
            foreach (var i in allItems.Where(
                a => a.MyActions
                    .OrderByDescending(c => c.UtcCreated)
                    .First()
                    .ThisAction == PullQueueItemActionEnum.Viewed)
                .ToList())
                if (i.MyActions.OrderByDescending(d => d.UtcCreated).First().UtcCreated < DateTime.UtcNow.AddMinutes(-60))
                    i.MyActions.Add(new PullQueueItemAction()
                    {
                        AuditEntityId = Guid.NewGuid(),
                        AuditEntityType = "System",
                        Comment = "Timeout",
                        Id = Guid.NewGuid(),
                        MyPullQueueItemId = i.Id,
                        ThisAction = PullQueueItemActionEnum.Opened,
                        UtcCreated = DateTime.UtcNow
                    });

            _itemRepo.Save();

            var openItems = allItems
                .Where(a => a.MyActions
                    .OrderByDescending(c => c.UtcCreated)
                    .First()
                    .ThisAction == PullQueueItemActionEnum.Opened
                    || a.MyActions
                    .OrderByDescending(c => c.UtcCreated)
                    .First()
                    .ThisAction == PullQueueItemActionEnum.Escalated)
                .OrderByDescending(oi => oi.UtcCreated)
                .ToList();

            return openItems;
        }

        public ICollection<PullQueueItem> GetOpenItems(Guid pullQueueId, int maxCount)
        {
            var allItems = _itemRepo
                .Query(i => i.MyPullQueueId == pullQueueId, _pullQueueItemIncludes);

            var openItems = allItems
                .Where(a => a.MyActions
                    .OrderByDescending(c => c.UtcCreated)
                    .First()
                    .ThisAction == PullQueueItemActionEnum.Opened
                    || a.MyActions
                    .OrderByDescending(c => c.UtcCreated)
                    .First()
                    .ThisAction == PullQueueItemActionEnum.Escalated)
                    .OrderByDescending(oi => oi.UtcCreated)
                    .Take(maxCount);

            return openItems.ToList();
        }

        public ICollection<PullQueueItem> GetItemsHavingResults(Guid pullQueueId)
        {
            var allItems = _itemRepo
                .Query(i => i.MyPullQueueId == pullQueueId, _pullQueueItemIncludes);

            return allItems
                .Where(a => a.MyActions
                    .Where(ma => ma.ThisAction == PullQueueItemActionEnum.Completed_ScoreCard)
                    .Any())
                .ToList();
        }

        public ICollection<PullQueueItem> GetOpenItemsHavingResults(Guid pullQueueId)
        {
            var allItems = _itemRepo
                .Query(i => i.MyPullQueueId == pullQueueId, _pullQueueItemIncludes);
            return allItems
                .Where(a =>
                    (
                        a.MyActions
                            .OrderByDescending(c => c.UtcCreated)
                            .First()
                            .ThisAction == PullQueueItemActionEnum.Opened
                        || a.MyActions
                            .OrderByDescending(c => c.UtcCreated)
                            .First()
                            .ThisAction == PullQueueItemActionEnum.Escalated
                    ) &&
                    a.MyActions
                        .Where(ma => ma.ThisAction == PullQueueItemActionEnum.Completed_ScoreCard)
                        .Any())
                .OrderByDescending(oi => oi.UtcCreated)
                .ToList();
        }

        public PullQueueItem GetItem(Guid pullQueueId, Guid itemId)
        {
            return _itemRepo
                .Query(i => i.MyPullQueueId == pullQueueId
                    && i.Id == itemId,
                    _pullQueueItemIncludes)
                .FirstOrDefault();
        }

        public void Item_AddToQueue(PullQueueItem theItem, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            //TODO: Validate new queue item!
            _itemRepo.Add(theItem);
            _itemRepo.Save();

            var newAction = theItem.CreateAction(PullQueueItemActionEnum.Opened, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_View(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var newAction = _itemRepo.GetById(itemId)
                .CreateAction(PullQueueItemActionEnum.Viewed, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_Assign(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var newAction = _itemRepo.GetById(itemId)
                .CreateAction(PullQueueItemActionEnum.Assigned, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_CompleteScoreCard(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var newAction = _itemRepo.GetById(itemId)
                .CreateAction(PullQueueItemActionEnum.Completed_ScoreCard, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_Escalate(Guid itemId, int newLevelOfConcern, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var theItem = _itemRepo.GetById(itemId);

            theItem.MinLevelOfConcern = newLevelOfConcern;

            _itemRepo.Update(theItem);
            _itemRepo.Save();

            var newAction = theItem.CreateAction(PullQueueItemActionEnum.Escalated, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_Flag(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var newAction = _itemRepo.GetById(itemId)
                .CreateAction(PullQueueItemActionEnum.Flagged, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_Remediate(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var newAction = _itemRepo.GetById(itemId)
                .CreateAction(PullQueueItemActionEnum.Remediated, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_Close(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var newAction = _itemRepo.GetById(itemId)
                .CreateAction(PullQueueItemActionEnum.Closed, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_Reopen(Guid itemId, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var newAction = _itemRepo.GetById(itemId)
                .CreateAction(PullQueueItemActionEnum.Opened, auditEntityType, auditEntityId, comment);

            _itemActionRepo.Add(newAction);
            _itemActionRepo.Save();
        }

        public void Item_Requeue(Guid itemId, Guid newQueueId, string auditEntityType, Guid auditEntityId, string comment = "")
        {
            var oldItem = _itemRepo.GetById(itemId);
            var newItem = new PullQueueItem();
            var newQueue = _pullQueueRepo.GetById(newQueueId);

            //TODO: Validate objects for item move

            newItem.MinLevelOfConcern = 0;
            newItem.MyActions.Add(newItem.CreateAction(PullQueueItemActionEnum.Opened, auditEntityType, auditEntityId, comment));
            newItem.MyWorkItemId = oldItem.MyWorkItemId;
            newItem.MyPullQueueId = newQueue.Id;

            _itemRepo.Add(newItem);
            _itemRepo.Save();
        }

    }
}