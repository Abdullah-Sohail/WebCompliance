using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Domain.ValueTypes;
using Compliance.Dashboard.Implementation.Ef.Maps;
using System.Configuration;
using Compliance.Dashboard.Domain.ValueType;

namespace Compliance.Dashboard.Implementation.Ef
{
    public class DashboardContext : DbContext
    {
        public DashboardContext() :base(ConfigurationManager.ConnectionStrings["ComplianceDashboardDb"].ConnectionString) { }

        public DashboardContext(string connString)
            : base(connString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            //System.Data.Entity.Database.SetInitializer<DashboardContext>(new DashboardContextInit());
        }

        public DbSet<DashProfile> DashProfiles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<QueueLevelConfig> QueueLevelConfigs { get; set; }
        public DbSet<QueueAssignment> QueueAssignments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new GroupMap());
            modelBuilder.Configurations.Add(new ProfileMap());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        //public class DashboardContextInit : DropCreateDatabaseIfModelChanges<DashboardContext>
        //{
        //    protected override void Seed(DashboardContext context)
        //    {
        //        //context.DashProfiles.Add(new DashProfile()
        //        //{
        //        //    ADUsername = "WACISSA\\ryanm",
        //        //    Id = Guid.Parse("B92E50A3-2905-4063-B2C9-6110E5A92DAC"),
        //        //    MyCellNumber = new Phone(8503046549),
        //        //    MyEmail = new EmailAddress("ryan.mack@divinitysoftware.com"),
        //        //    Name = "Ryan Mack",
        //        //    UtcCreated = DateTime.UtcNow
        //        //}); 
                
        //        //context.DashProfiles.Add(new DashProfile()
        //        //{
        //        //    ADUsername = "WACISSA\\jime",
        //        //    Id = Guid.Parse("5AE93E52-43F0-460A-B1A7-604C099FBF2D"),
        //        //    MyCellNumber = new Phone(9045551212),
        //        //    MyEmail = new EmailAddress("jim.ecclestopn@divinitysoftware.com"),
        //        //    Name = "Jim Eccleston",
        //        //    UtcCreated = DateTime.UtcNow
        //        //});

        //        //context.SaveChanges();

        //        //var teamId = Guid.Parse("165278C1-7CF0-461C-B008-CC209ADA7A62");
        //        //var groupId = Guid.NewGuid();

        //        //var newTeam = new Team()
        //        //{
        //        //    Id = teamId,
        //        //    TeamName = "PhoneAudit",
        //        //    UtcCreated = DateTime.UtcNow,
        //        //    Groups = new List<Group>() { 
        //        //        new Group(){
        //        //            Id = Guid.NewGuid(),
        //        //            GroupName = "Auditors",
        //        //            GroupType = Common.Enums.GroupTypeEnum.Auditor,
        //        //            LevelOfConcern = 1,
        //        //            MyTeamId = teamId,
        //        //            UtcCreated = DateTime.UtcNow,
        //        //            GroupMembers = new List<GroupMember>()
        //        //        },
        //        //        new Group(){
        //        //            Id = Guid.NewGuid(),
        //        //            GroupName = "Basic Review",
        //        //            GroupType = Common.Enums.GroupTypeEnum.Review,
        //        //            LevelOfConcern = 2,
        //        //            MyTeamId = teamId,
        //        //            UtcCreated = DateTime.UtcNow,
        //        //            GroupMembers = new List<GroupMember>()
        //        //        },
        //        //        new Group(){
        //        //            Id = groupId,
        //        //            GroupName = "Extended Review",
        //        //            GroupType = Common.Enums.GroupTypeEnum.Review,
        //        //            LevelOfConcern = 5,
        //        //            MyTeamId = teamId,
        //        //            UtcCreated = DateTime.UtcNow,
        //        //            GroupMembers = new List<GroupMember>(){ new GroupMember() { 
        //        //                Id = Guid.NewGuid(),
        //        //                MyDashProfileId = Guid.Parse("B92E50A3-2905-4063-B2C9-6110E5A92DAC"),
        //        //                MyGroupId = groupId,
        //        //                UtcCreated = DateTime.UtcNow
        //        //                },
        //        //                new GroupMember() { 
        //        //                Id = Guid.NewGuid(),
        //        //                MyDashProfileId = Guid.Parse("5AE93E52-43F0-460A-B1A7-604C099FBF2D"),
        //        //                MyGroupId = groupId,
        //        //                UtcCreated = DateTime.UtcNow
        //        //                }
        //        //            },
        //        //        }
        //        //    },
        //        //    ScoreCardAssignments = new List<ScoreCardAssignment>() { 
        //        //        new ScoreCardAssignment(){
        //        //            Id = Guid.NewGuid(),
        //        //            MyTeamId = teamId,
        //        //            MyScoreCardId = Guid.Parse("BA0B0472-55B4-427C-9CC1-F254690949D7"),
        //        //            MinLevelOfConcern = 1,
        //        //            Active = true,
        //        //            UtcCreated = DateTime.UtcNow
        //        //        }
        //        //    }
        //        //};

        //        //teamId = Guid.Parse("DDF55B14-3808-43D3-88F4-31447F1D4A10");
        //        //groupId = Guid.NewGuid();
                
        //        //var adminTeam = new Team()
        //        //{
        //        //    Id = teamId,
        //        //    TeamName = "Administration",
        //        //    UtcCreated = DateTime.UtcNow,
        //        //    Groups = new List<Group>() {
        //        //        new Group(){
        //        //            Id = groupId,
        //        //            GroupName = "Site Admins",
        //        //            GroupType = Common.Enums.GroupTypeEnum.Admin,
        //        //            LevelOfConcern = 999,
        //        //            MyTeamId = teamId,
        //        //            UtcCreated = DateTime.UtcNow,
        //        //            GroupMembers = new List<GroupMember>(){ new GroupMember() { 
        //        //                Id = Guid.NewGuid(),
        //        //                MyDashProfileId = Guid.Parse("B92E50A3-2905-4063-B2C9-6110E5A92DAC"),
        //        //                MyGroupId = groupId,
        //        //                UtcCreated = DateTime.UtcNow
        //        //                },
        //        //                new GroupMember() { 
        //        //                Id = Guid.NewGuid(),
        //        //                MyDashProfileId = Guid.Parse("5AE93E52-43F0-460A-B1A7-604C099FBF2D"),
        //        //                MyGroupId = groupId,
        //        //                UtcCreated = DateTime.UtcNow
        //        //                }
        //        //            },
        //        //        }
        //        //    },
        //        //    ScoreCardAssignments = new List<ScoreCardAssignment>()
        //        //};

        //        //context.Teams.Add(newTeam);
        //        //context.Teams.Add(adminTeam);

        //        //context.SaveChanges();

        //        ////Default "FirstQueue"
        //        //var newQueueId = Guid.Parse("AD182134-5FF5-4507-9A98-4261BFA1AC06");

        //        //context.QueueLevelConfigs.Add(new QueueLevelConfig()
        //        //{
        //        //    Id = Guid.NewGuid(),
        //        //    QueueId = newQueueId,
        //        //    QueueLevel = 1,
        //        //    MenuName = "New Items",
        //        //    ViewTimeout = 15,
        //        //    AssignTimeout = 60,
        //        //    ExtendMinutes = 60,
        //        //    SupervisorLevel = 101,
        //        //    ActionName = "Work",
        //        //    UtcCreated = DateTime.UtcNow
        //        //});
        //        //context.QueueLevelConfigs.Add(new QueueLevelConfig()
        //        //{
        //        //    Id = Guid.NewGuid(),
        //        //    QueueId = newQueueId,
        //        //    QueueLevel = 2,
        //        //    MenuName = "Basic Review",
        //        //    ViewTimeout = 15,
        //        //    AssignTimeout = 60,
        //        //    ExtendMinutes = 60,
        //        //    SupervisorLevel = 102,
        //        //    ActionName = "Review",
        //        //    UtcCreated = DateTime.UtcNow
        //        //});
        //        //context.QueueLevelConfigs.Add(new QueueLevelConfig()
        //        //{
        //        //    Id = Guid.NewGuid(),
        //        //    QueueId = newQueueId,
        //        //    QueueLevel = 5,
        //        //    MenuName = "Extended Review",
        //        //    ViewTimeout = 15,
        //        //    AssignTimeout = 60,
        //        //    ExtendMinutes = 60,
        //        //    SupervisorLevel = 105,
        //        //    ActionName = "Review",
        //        //    UtcCreated = DateTime.UtcNow
        //        //});
        //        //context.QueueLevelConfigs.Add(new QueueLevelConfig()
        //        //{
        //        //    Id = Guid.NewGuid(),
        //        //    QueueId = newQueueId,
        //        //    QueueLevel = 101,
        //        //    MenuName = "Score Cards",
        //        //    ViewTimeout = 15,
        //        //    AssignTimeout = 60,
        //        //    ExtendMinutes = 60,
        //        //    SupervisorLevel = 0,
        //        //    ActionName = "Audit",
        //        //    UtcCreated = DateTime.UtcNow
        //        //});
        //        //context.QueueLevelConfigs.Add(new QueueLevelConfig()
        //        //{
        //        //    Id = Guid.NewGuid(),
        //        //    QueueId = newQueueId,
        //        //    QueueLevel = 102,
        //        //    MenuName = "Basic Review",
        //        //    ViewTimeout = 15,
        //        //    AssignTimeout = 60,
        //        //    ExtendMinutes = 60,
        //        //    SupervisorLevel = 0,
        //        //    ActionName = "Audit",
        //        //    UtcCreated = DateTime.UtcNow
        //        //});
        //        //context.QueueLevelConfigs.Add(new QueueLevelConfig()
        //        //{
        //        //    Id = Guid.NewGuid(),
        //        //    QueueId = newQueueId,
        //        //    QueueLevel = 105,
        //        //    MenuName = "Extended Review",
        //        //    ViewTimeout = 15,
        //        //    AssignTimeout = 60,
        //        //    ExtendMinutes = 60,
        //        //    SupervisorLevel = 0,
        //        //    ActionName = "Audit",
        //        //    UtcCreated = DateTime.UtcNow
        //        //});

        //        //context.SaveChanges();

        //    }
        //}
    }
}
