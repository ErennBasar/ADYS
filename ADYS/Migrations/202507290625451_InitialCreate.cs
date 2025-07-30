namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdvisorAssignments",
                c => new
                    {
                        AdvisorAssignmentId = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: false),
                        AdvisorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdvisorAssignmentId)
                .ForeignKey("dbo.Users", t => t.AdvisorId)
                .ForeignKey("dbo.Users", t => t.StudentId)
                .Index(t => t.StudentId)
                .Index(t => t.AdvisorId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleteDate = c.DateTime(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.CourseSelections",
                c => new
                    {
                        CourseSelectionId = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        IsApprovedByAdvisor = c.Boolean(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.CourseSelectionId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.StudentId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.StudentId)
                .Index(t => t.CourseId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        CourseName = c.String(),
                        AKTS = c.Int(nullable: false),
                        Kontenjan = c.Int(nullable: false),
                        AdvisorId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        DayOfWeek = c.String(),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dbo.Users", t => t.AdvisorId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.AdvisorId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserRoleId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserRoleId)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.Terms",
                c => new
                    {
                        TermId = c.Int(nullable: false, identity: true),
                        TermName = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CourseSelectionStart = c.DateTime(),
                        CourseSelectionEnd = c.DateTime(),
                    })
                .PrimaryKey(t => t.TermId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvisorAssignments", "StudentId", "dbo.Users");
            DropForeignKey("dbo.AdvisorAssignments", "AdvisorId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.CourseSelections", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.CourseSelections", "StudentId", "dbo.Users");
            DropForeignKey("dbo.Courses", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.CourseSelections", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Courses", "AdvisorId", "dbo.Users");
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.Courses", new[] { "DepartmentId" });
            DropIndex("dbo.Courses", new[] { "AdvisorId" });
            DropIndex("dbo.CourseSelections", new[] { "User_UserId" });
            DropIndex("dbo.CourseSelections", new[] { "CourseId" });
            DropIndex("dbo.CourseSelections", new[] { "StudentId" });
            DropIndex("dbo.AdvisorAssignments", new[] { "AdvisorId" });
            DropIndex("dbo.AdvisorAssignments", new[] { "StudentId" });
            DropTable("dbo.Terms");
            DropTable("dbo.Roles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Departments");
            DropTable("dbo.Courses");
            DropTable("dbo.CourseSelections");
            DropTable("dbo.Users");
            DropTable("dbo.AdvisorAssignments");
        }
    }
}
