using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Queuing.Implementation.Ef
{
    public class QueueContextFromConfig : IMakeDbContext<QueuingContext>
    {
        public DbContext GetContext()
        {
            return new QueuingContext(ConfigurationManager.ConnectionStrings["ComplianceQueuingDb"].ConnectionString);
        }
    }
}
