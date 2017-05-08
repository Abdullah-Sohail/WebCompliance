using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Implementation.Ef;

namespace Dashboard.Tests.Service.Spikes.Ef
{
    public class TestContext : IMakeDbContext<DashboardContext>
    {
        private const string _connString =
             "Data Source=localhost\\sqlexpress;Initial Catalog=ComplianceDashboard;Integrated Security=SSPI;";

        public System.Data.Entity.DbContext GetContext()
        {
            return new DashboardContext(_connString);
        }
    }
}
