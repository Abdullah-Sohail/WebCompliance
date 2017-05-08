using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Audio.Domain.ValueTypes;

namespace Compliance.Audio.Implementation.Ef.Persistence.Data.Maps
{
    public class ResultReferenceMap : EntityTypeConfiguration<ResultReference>
    {
        public ResultReferenceMap()
        {
            this.HasKey(a => new { a.MyRecordingId, a.ResultCode });
        }
    }
}