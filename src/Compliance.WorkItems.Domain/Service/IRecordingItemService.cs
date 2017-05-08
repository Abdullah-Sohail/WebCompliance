using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.WorkItems.Domain.ValueTypes;

namespace Compliance.WorkItems.Domain.Service
{
    public interface IRecordingItemService
    {
        RecordingItem GetById(Guid id);
        RecordingItem Add(RecordingItem newItem);

        void UpdateAllRelationships(RecordingItem item);
        void AddRelationship<T>(Guid recordingItemId, T r) where T : Relationship;
    }
}
