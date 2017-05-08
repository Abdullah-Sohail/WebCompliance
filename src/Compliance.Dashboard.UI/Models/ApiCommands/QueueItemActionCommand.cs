using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Compliance.Dashboard.UI.Models.ApiCommands
{
    public class QueueItemActionCommand
    {
        public Guid ItemId { get; set; }
        public string AuditEntityType { get; set; }
        public Guid AuditEntityId { get; set; }
        public string Comment { get; set; }
        public string ActionString { get; set; }
        public int? NewLevel { get; set; }
    }
}