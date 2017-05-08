using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.WorkItems.Domain;

namespace Compliance.WorkItems.Implementation.Ef.Maps
{
    public class RecordingItemMap : EntityTypeConfiguration<RecordingItem>
    {
        public RecordingItemMap()
        {
            this.HasMany(r => r.Files)
                .WithOptional()
                .HasForeignKey(f => f.MyWorkItemId)
                .WillCascadeOnDelete(false);

            HasMany(r => r.PersonRelationships)
                .WithOptional()
                .HasForeignKey(p => p.MyWorkItemId)
                .WillCascadeOnDelete(true);

            HasMany(r => r.AccountRelationships)
                .WithOptional()
                .HasForeignKey(p => p.MyWorkItemId)
                .WillCascadeOnDelete(true);

            HasMany(r => r.ClientRelationships)
                .WithOptional()
                .HasForeignKey(p => p.MyWorkItemId)
                .WillCascadeOnDelete(true);

            HasMany(r => r.CompanyRelationships)
                .WithOptional()
                .HasForeignKey(p => p.MyWorkItemId)
                .WillCascadeOnDelete(true);

            HasMany(r => r.DeskRelationships)
                .WithOptional()
                .HasForeignKey(p => p.MyWorkItemId)
                .WillCascadeOnDelete(true);

            HasMany(r => r.PhoneRelationships)
                .WithOptional()
                .HasForeignKey(p => p.MyWorkItemId)
                .WillCascadeOnDelete(true);

            HasMany(r => r.WorkerRelationships)
                .WithOptional()
                .HasForeignKey(p => p.MyWorkItemId)
                .WillCascadeOnDelete(true);
        }
    }
}
