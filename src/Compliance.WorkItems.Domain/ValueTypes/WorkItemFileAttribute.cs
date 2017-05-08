using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.WorkItems.Domain.ValueTypes
{
    public class WorkItemFileAttribute
    {
        public Guid Id { get; set; }
        public Guid MyWorkItemFileId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }

        public WorkItemFile MyWorkItemFile { get; set; }
    }
}
