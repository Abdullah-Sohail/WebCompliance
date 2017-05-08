using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.WorkItems.Domain.ValueTypes;

namespace Compliance.WorkItems.Domain
{
    public class RecordingItem : WorkItem, ICanGenericRepo
    {
        public string OurNumber { get; set; }
        public string TheirNumber { get; set; }
        public bool Incoming { get; set; }
        public DateTime CallDateTime { get; set; }
        public int CallDuration { get; set; }
        public Guid? MyRecordingFileId { get; set; }

        public WorkItemFile MyRecordingFile { get; set; }
    }
}
