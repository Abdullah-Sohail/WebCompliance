using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Queuing.Implementation.Ef;

namespace Queuing.Tests.Service.Spikes.Ef.Config
{
    public class TestingContext : IMakeDbContext<QueuingContext>
    {
        private const string _connString =
             "Data Source=localhost\\sqlexpress;Integrated Security=SSPI;Initial Catalog=ComplianceQueuing;";

        public System.Data.Entity.DbContext GetContext()
        {
            return new QueuingContext(_connString);
        }
    }
}
