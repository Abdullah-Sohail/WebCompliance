using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.WorkItems.Implementation.Ef
{
    public class WorkItemsContextFromConfig : IMakeDbContext<WorkItemsContext>
    {
        public DbContext GetContext()
        {
            return new WorkItemsContext(ConfigurationManager.ConnectionStrings["ComplianceWorkItemsDb"].ConnectionString);
        }
    }
}
