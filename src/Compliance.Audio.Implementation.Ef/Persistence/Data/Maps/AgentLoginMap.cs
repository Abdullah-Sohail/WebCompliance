using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Audio.Domain;

namespace Compliance.Audio.Implementation.Ef.Persistence.Data.Maps
{
    public class AgentLoginMap : EntityTypeConfiguration<AgentLogin>
    {
        public AgentLoginMap()
        {
            this.HasKey(a => a.UserLogin);
        }
    }
}