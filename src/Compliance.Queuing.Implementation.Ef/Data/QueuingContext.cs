using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.Enums;
using Compliance.Queuing.Domain;
using Compliance.Queuing.Domain.ValueTypes;

namespace Compliance.Queuing.Implementation.Ef
{
    public class QueuingContext : DbContext
    {
        public QueuingContext(string connString)
            : base(connString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            System.Data.Entity.Database.SetInitializer(new QueuingContextInit());
        }

        public DbSet<PullQueue> PullQueues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Configurations.Add(new PullQueueMap());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public class QueuingContextInit : DropCreateDatabaseIfModelChanges<QueuingContext>
        {
            protected override void Seed(QueuingContext context)
            {

                var newQueue = new PullQueue() {
                    Id = Guid.Parse("AD182134-5FF5-4507-9A98-4261BFA1AC06"),
                    QueueName = "FirstQueue",
                    WorkItemType = WorkItemTypeEnum.RecordingItem,
                    UtcCreated = DateTime.UtcNow,
                    Active = true
                };

                var itemId = Guid.NewGuid();

                newQueue.MyPullQueueItems = new List<PullQueueItem>() { 
                    new PullQueueItem(){
                        Id = itemId,
                        MinLevelOfConcern = 1,
                        MyPullQueueId = Guid.Parse("AD182134-5FF5-4507-9A98-4261BFA1AC06"),
                        MyWorkItemId = Guid.Parse("2A33ADA7-C5B7-440A-A900-94D5915C7425"),
                        UtcCreated = DateTime.UtcNow,
                        MyActions = new List<PullQueueItemAction>()
                    }
                };

                var initialAction = newQueue.MyPullQueueItems.ToList()[0]
                    .CreateAction(PullQueueItemActionEnum.Opened, "DomainUser", 
                    Guid.Parse("B92E50A3-2905-4063-B2C9-6110E5A92DAC"), "From Seed");

                newQueue.MyPullQueueItems.ToList()[0].MyActions.Add(initialAction);

                context.PullQueues.Add(newQueue);
                context.SaveChanges();
            }
        }
    }
}
