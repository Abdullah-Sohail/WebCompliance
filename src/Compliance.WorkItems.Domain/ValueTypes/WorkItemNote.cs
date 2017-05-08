using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.WorkItems.Domain.ValueTypes
{
    public class WorkItemNote
    {
        public Guid Id { get; set; }
        public Guid MyWorkItemId { get; set; }
        public string Comment { get; set; }
        public string AuditEntityType { get; set; }
        public Guid AuditEntityId { get; set; }
        public DateTime UtcCreated { get; set; }
    }
}
