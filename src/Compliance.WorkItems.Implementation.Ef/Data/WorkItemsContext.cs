using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.WorkItems.Domain;
using Compliance.WorkItems.Domain.ValueTypes;
using Compliance.WorkItems.Implementation.Ef.Maps;

namespace Compliance.WorkItems.Implementation.Ef
{
    public class WorkItemsContext : DbContext
    {
        public WorkItemsContext(string connString)
            : base(connString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            System.Data.Entity.Database.SetInitializer(new WorkItemsContextInit());
        }

        public DbSet<RecordingItem> RecordingItems { get; set; }
        public DbSet<WorkItemFile> WorkItemFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new RecordingItemMap());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public class WorkItemsContextInit : DropCreateDatabaseIfModelChanges<WorkItemsContext>
        {
            protected override void Seed(WorkItemsContext context)
            {
                var itemId = Guid.Parse("2A33ADA7-C5B7-440A-A900-94D5915C7425");

                var newItem = new RecordingItem() { 
                    Id = itemId,
                    LogicalIdentifier = "SampleRecording",
                    OurNumber = "8005551212",
                    TheirNumber = "8503046549",
                    Incoming = true,
                    CallDuration = 145,
                    CallDateTime = DateTime.UtcNow.AddDays(-2),
                    UtcCreated = DateTime.UtcNow,
                    Files = new List<WorkItemFile>()
                };

                var theFile = new WorkItemFile() { 
                    Id = Guid.NewGuid(),
                    MyWorkItemId = itemId,
                    MinLevelOfConcern = 1,
                    MediaType = "application/mp3",
                    FilePath = "c:\\Temp\\",
                    FileName = "recording.mp3",
                    Expires = DateTime.UtcNow.AddDays(5),
                    UtcCreated = DateTime.UtcNow
                };

                newItem.Files.Add(theFile);

                context.RecordingItems.Add(newItem);
                
                context.SaveChanges();

                newItem.MyRecordingFile = theFile;

                context.SaveChanges();

            }
        }
    }
}
