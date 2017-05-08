namespace Compliance.Dashboard.Implementation.Ef.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DashProfiles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 256),
                        LastName = c.String(),
                        Email_Domain = c.String(),
                        Email_Username = c.String(),
                        CellNumber_Value = c.Long(),
                        UtcCreated = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MyTeamId = c.Guid(nullable: false),
                        GroupName = c.String(nullable: false, maxLength: 60),
                        GroupType = c.Int(nullable: false),
                        LevelOfConcern = c.Int(nullable: false),
                        LevelLock = c.Boolean(nullable: false),
                        UtcCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.MyTeamId)
                .Index(t => t.MyTeamId);
            
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MyDashProfileId = c.Guid(nullable: false),
                        MyGroupId = c.Guid(nullable: false),
                        UtcCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DashProfiles", t => t.MyDashProfileId)
                .ForeignKey("dbo.Groups", t => t.MyGroupId)
                .Index(t => t.MyDashProfileId)
                .Index(t => t.MyGroupId);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TeamName = c.String(),
                        UtcCreated = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QueueAssignments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MyTeamId = c.Guid(nullable: false),
                        MyQueueId = c.Guid(nullable: false),
                        UtcCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.MyTeamId)
                .Index(t => t.MyTeamId);
            
            CreateTable(
                "dbo.ScoreCardAssignments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MyTeamId = c.Guid(nullable: false),
                        MyScoreCardId = c.Guid(nullable: false),
                        MinLevelOfConcern = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        UtcCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.MyTeamId)
                .Index(t => t.MyTeamId);
            
            CreateTable(
                "dbo.QueueLevelConfigs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        QueueId = c.Guid(nullable: false),
                        QueueLevel = c.Int(nullable: false),
                        MenuName = c.String(),
                        SupervisorLevel = c.Int(nullable: false),
                        ViewTimeout = c.Int(nullable: false),
                        AssignTimeout = c.Int(nullable: false),
                        ExtendMinutes = c.Int(nullable: false),
                        ActionName = c.String(),
                        UtcCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoreCardAssignments", "MyTeamId", "dbo.Teams");
            DropForeignKey("dbo.QueueAssignments", "MyTeamId", "dbo.Teams");
            DropForeignKey("dbo.Groups", "MyTeamId", "dbo.Teams");
            DropForeignKey("dbo.GroupMembers", "MyGroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupMembers", "MyDashProfileId", "dbo.DashProfiles");
            DropIndex("dbo.ScoreCardAssignments", new[] { "MyTeamId" });
            DropIndex("dbo.QueueAssignments", new[] { "MyTeamId" });
            DropIndex("dbo.Groups", new[] { "MyTeamId" });
            DropIndex("dbo.GroupMembers", new[] { "MyGroupId" });
            DropIndex("dbo.GroupMembers", new[] { "MyDashProfileId" });
            DropTable("dbo.QueueLevelConfigs");
            DropTable("dbo.ScoreCardAssignments");
            DropTable("dbo.QueueAssignments");
            DropTable("dbo.Teams");
            DropTable("dbo.GroupMembers");
            DropTable("dbo.Groups");
            DropTable("dbo.DashProfiles");
        }
    }
}
