using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Compliance.WorkItems.Domain.ValueTypes;

namespace Compliance.Dashboard.UI.Models.ApiCommands
{
    public class AddWorkerRelationshipsCommand
    {
        public ICollection<WorkerRelation> Relationships { get; set; }
    }
}