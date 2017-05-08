using System.Configuration;
using System.Data.Entity;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Dashboard.Implementation.Ef
{
    public class DashContextFromConfig : IMakeDbContext<DashboardContext>
    {
        public DbContext GetContext()
        {
            return new DashboardContext(ConfigurationManager.ConnectionStrings["ComplianceDashboardDb"].ConnectionString);
        }
    }
}
