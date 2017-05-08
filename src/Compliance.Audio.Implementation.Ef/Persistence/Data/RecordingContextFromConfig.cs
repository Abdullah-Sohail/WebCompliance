using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Audio.Implementation.Ef.Persistence.Data
{
    public class RecordingContextFromConfig : IMakeDbContext<RecordingContext>
    {
        public DbContext GetContext()
        {
            return new RecordingContext(ConfigurationManager.ConnectionStrings["CallInfoDb"].ConnectionString);
        }
    }
}
