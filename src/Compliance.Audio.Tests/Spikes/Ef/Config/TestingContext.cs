using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Audio.Implementation.Ef.Persistence.Data;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Audio.Tests.Spikes.Ef.Config
{
    public class TestingContext : IMakeDbContext<RecordingContext>
    {
        private string _connString =
             "Data Source=localhost\\sqlexpress;Initial Catalog=CallInfo;Integrated Security=SSPI;";

        public DbContext GetContext()
        {
            return new RecordingContext(_connString);
        }
    }
}
