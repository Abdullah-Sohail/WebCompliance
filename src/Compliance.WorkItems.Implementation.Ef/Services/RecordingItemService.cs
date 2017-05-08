using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.WorkItems.Domain;
using Compliance.WorkItems.Domain.Service;
using Compliance.WorkItems.Domain.ValueTypes;

namespace Compliance.WorkItems.Implementation.Ef.Services
{
    public class RecordingItemService : IRecordingItemService
    {

        private IGenericRepo<RecordingItem, WorkItemsContext> _rItemRepo;

        private string[] _rItemIncludes = new string[] { "MyRecordingFile", "Files", 
            "AccountRelationships", "ClientRelationships", "CompanyRelationships", 
            "DeskRelationships", "PersonRelationships", "PhoneRelationships", "WorkerRelationships" };

        public RecordingItemService(IGenericRepo<RecordingItem, WorkItemsContext> rItemRepo)
        {
            _rItemRepo = rItemRepo;
        }

        public RecordingItem GetById(Guid id)
        {
            return _rItemRepo
                .Query(r => r.Id == id, _rItemIncludes)
                .FirstOrDefault();
        }

        public RecordingItem Add(RecordingItem newItem)
        {
            Guid? rfId = null;

            if (newItem.Id == Guid.Empty)
                newItem.Id = Guid.NewGuid();

            foreach (var f in newItem.Files)
            {
                if (f.Id == Guid.Empty)
                    f.Id = Guid.NewGuid();
                f.MyWorkItemId = newItem.Id;
            }

            //We need to save the new item before we can link it up as "MyRecording"
            rfId = newItem.MyRecordingFileId;
            newItem.MyRecordingFile = null;
            newItem.MyRecordingFileId = null;

            _rItemRepo.Add(newItem);
            _rItemRepo.Save();

            if (rfId.HasValue && rfId.Value != Guid.Empty)
            {
                newItem.MyRecordingFileId = rfId;
                _rItemRepo.Update(newItem);
                _rItemRepo.Save();
            }

            return newItem;
        }

        public void UpdateAllRelationships(RecordingItem item)
        {
            var oldItem = GetById(item.Id);

            oldItem.AccountRelationships = item.AccountRelationships;
            oldItem.ClientRelationships = item.ClientRelationships;
            oldItem.CompanyRelationships = item.CompanyRelationships;
            oldItem.DeskRelationships = item.DeskRelationships;
            oldItem.PersonRelationships = item.PersonRelationships;
            oldItem.PhoneRelationships = item.PhoneRelationships;
            oldItem.WorkerRelationships = item.WorkerRelationships;

            _rItemRepo.Update(oldItem);
            _rItemRepo.Save();

        }


        public void AddRelationship<T>(Guid workItemId, T r)
            where T : Relationship
        {
            var theItem = GetById(workItemId);
            var namespaces = r.GetType().ToString().Split('.');
            var ns = namespaces[namespaces.GetUpperBound(0)];

            if (r.Id == Guid.Empty)
                r.Id = Guid.NewGuid();

            if (r.MyWorkItemId == Guid.Empty)
                r.MyWorkItemId = workItemId;

            if (r.MyWorkItemId != workItemId)
                throw new Exception("Mismatch Work Item ID's");

            r.UtcCreated = DateTime.UtcNow;

            switch (ns)
            {
                case "AccountRelation":
                    if (!theItem.AccountRelationships.Where(x => x.Identifier == r.Identifier).Any())
                        theItem.AccountRelationships.Add(r as AccountRelation);
                    break;
                case "ClientRelation":
                    if (!theItem.ClientRelationships.Where(x => x.Identifier == r.Identifier).Any())
                        theItem.ClientRelationships.Add(r as ClientRelation);
                    break;
                case "CompanyRelation":
                    if (!theItem.CompanyRelationships.Where(x => x.Identifier == r.Identifier).Any())
                        theItem.CompanyRelationships.Add(r as CompanyRelation);
                    break;
                case "DeskRelation":
                    if (!theItem.DeskRelationships.Where(x => x.Identifier == r.Identifier).Any())
                        theItem.DeskRelationships.Add(r as DeskRelation);
                    break;
                case "PersonRelation":
                    if (!theItem.PersonRelationships.Where(x => x.Identifier == r.Identifier).Any())
                        theItem.PersonRelationships.Add(r as PersonRelation);
                    break;
                case "PhoneRelation":
                    if (!theItem.PhoneRelationships.Where(x => x.Identifier == r.Identifier).Any())
                        theItem.PhoneRelationships.Add(r as PhoneRelation);
                    break;
                case "WorkerRelation":
                    if (!theItem.WorkerRelationships.Where(x => x.Identifier == r.Identifier).Any())
                        theItem.WorkerRelationships.Add(r as WorkerRelation);
                    break;
                default:
                    throw new Exception(String.Format("Cannot add relationship of type '{0}'", ns));
            }

            _rItemRepo.Update(theItem);
            _rItemRepo.Save();

        }
    }
}
