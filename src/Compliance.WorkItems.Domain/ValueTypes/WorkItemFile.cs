using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.WorkItems.Domain.ValueTypes
{
    public class WorkItemFile
    {
        public Guid Id { get; set; }
        public Guid? MyWorkItemId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Comment { get; set; }
        public string MediaType { get; set; }
        public int MinLevelOfConcern { get; set; }

        public ICollection<WorkItemFileAttribute> MyAttributes { get; set; }

        public DateTime Expires { get; set; }
        public DateTime UtcCreated { get; set; }
    }
}
