using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Dashboard.Domain;

namespace Compliance.Dashboard.Implementation.Ef.Maps
{
    public class GroupMap : EntityTypeConfiguration<Group>
    {
        public GroupMap()
        {

            this.Property(g => g.GroupName)
                .HasMaxLength(60)
                .IsRequired();
        }
    }
}
