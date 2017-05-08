using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Dashboard.Domain;

namespace Compliance.Dashboard.Implementation.Ef.Maps
{
    public class ProfileMap : EntityTypeConfiguration<DashProfile>
    {
        public ProfileMap()
        {
            this.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(256);
            
        }
    }
}
