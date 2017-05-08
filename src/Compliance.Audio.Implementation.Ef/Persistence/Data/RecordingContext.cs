using System;
using System.Collections.Generic;
using System.Data.Entity;
using Compliance.Audio.Domain;
using Compliance.Audio.Domain.ValueTypes;
using Compliance.Audio.Implementation.Ef.Persistence.Data.Maps;

namespace Compliance.Audio.Implementation.Ef.Persistence.Data
{
    public class RecordingContext : DbContext
    {
        public RecordingContext(string connString)
            : base(connString)
        {
            Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer(new RecordingContextInit());
        }

        public DbSet<Recording> Recordings { get; set; }
        public DbSet<AgentLogin> AgentLogins { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new AccountReferenceMap());
            modelBuilder.Configurations.Add(new CustomerReferenceMap());
            modelBuilder.Configurations.Add(new DeskReferenceMap());
            modelBuilder.Configurations.Add(new UserReferenceMap());
            modelBuilder.Configurations.Add(new ResultReferenceMap());
            modelBuilder.Configurations.Add(new AgentLoginMap());

        }
    }

    public class RecordingContextInit : DropCreateDatabaseIfModelChanges<RecordingContext>
    {
        protected override void Seed(RecordingContext context)
        {
            context.AgentLogins.Add(new AgentLogin
            {
                UserLogin = "AgentSmith"
            });

            context.AgentLogins.Add(new AgentLogin
            {
                UserLogin = "AgentJane"
            });

            var theId = Guid.Parse("88A96F46-8225-4799-9F13-1F5145814545");

            context.Recordings.Add(new Recording
            {
                AccountReferences = new List<AccountReference>
                {
                    new AccountReference
                    {
                        AccountNumber = 12345,
                        MyRecordingId = theId
                    }
                },
                ArchiveFilename = null,
                CallDuration = 120,
                CallIdentity = "CDR1234",
                CallStart = DateTime.Parse("1/1/2016 18:00"),
                CustomerReferences = new List<CustomerReference>
                {
                    new CustomerReference
                    {
                        MyRecordingId = theId,
                        CustomerCode = "DTV0001"
                    }
                },
                DeskReferences = new List<DeskReference>
                {
                    new DeskReference
                    {
                        MyRecordingId = theId,
                        DeskCode = "DESK3"
                    }
                },
                Filename = "C:\\Temp\\demorecording.mp3",
                Id = theId,
                Incoming = true,
                OurNumber = "9045551212",
                ResultReferences = new List<ResultReference>
                {
                    new ResultReference
                    {
                        MyRecordingId = theId,
                        ResultCode = "CO"
                    }
                },
                TheirNumber = "8503046549",
                UserReferences = new List<UserReference>
                {
                    new UserReference
                    {
                        MyRecordingId = theId,
                        UserLogin = "AgentSmith"
                    }
                }
            });

            theId = Guid.NewGuid();

            context.Recordings.Add(new Recording
            {
                AccountReferences = new List<AccountReference>
                {
                    new AccountReference
                    {
                        AccountNumber = 54321,
                        MyRecordingId = theId
                    }
                },
                ArchiveFilename = null,
                CallDuration = 120,
                CallIdentity = "CDR4321",
                CallStart = DateTime.Parse("10/1/2016 18:00"),
                CustomerReferences = new List<CustomerReference>
                {
                    new CustomerReference
                    {
                        MyRecordingId = theId,
                        CustomerCode = "COMCAST"
                    }
                },
                DeskReferences = new List<DeskReference>
                {
                    new DeskReference
                    {
                        MyRecordingId = theId,
                        DeskCode = "NEWBIZ"
                    }
                },
                Filename = "C:\\Temp\\demorecording.mp3",
                Id = theId,
                Incoming = true,
                OurNumber = "9045551212",
                ResultReferences = new List<ResultReference>
                {
                    new ResultReference
                    {
                        MyRecordingId = theId,
                        ResultCode = "CO"
                    }
                },
                TheirNumber = "2155554254",
                UserReferences = new List<UserReference>
                {
                    new UserReference
                    {
                        MyRecordingId = theId,
                        UserLogin = "AgentJane"
                    }
                }
            });

            context.SaveChanges();
        }
    }
}
