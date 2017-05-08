using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.WorkItems.Implementation.Ef;

namespace WorkItems.Tests.Service.Spikes.Ef.Config
{
    public class TestingContext : IMakeDbContext<WorkItemsContext>
    {
        private const string _connString =
             "Data Source=localhost\\sqlexpress;Initial Catalog=ComplianceWorkItems;Integrated Security=SSPI;";

        public System.Data.Entity.DbContext GetContext()
        {
            return new WorkItemsContext(_connString);
        }
    }
}
