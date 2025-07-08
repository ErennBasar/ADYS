namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Advisors",
                c => new
                    {
                        AdvisorId = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.AdvisorId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentId = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        AdvisorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StudentId)
                .ForeignKey("dbo.Advisors", t => t.AdvisorId, cascadeDelete: true)
                .Index(t => t.AdvisorId);
            
            CreateTable(
                "dbo.CourseSelections",
                c => new
                    {
                        CourseSelectionId = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        IsApprovedByAdvisor = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CourseSelectionId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.StudentId)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        CourseName = c.String(),
                        AKTS = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseSelections", "StudentId", "dbo.Students");
            DropForeignKey("dbo.CourseSelections", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Students", "AdvisorId", "dbo.Advisors");
            DropIndex("dbo.CourseSelections", new[] { "CourseId" });
            DropIndex("dbo.CourseSelections", new[] { "StudentId" });
            DropIndex("dbo.Students", new[] { "AdvisorId" });
            DropTable("dbo.Courses");
            DropTable("dbo.CourseSelections");
            DropTable("dbo.Students");
            DropTable("dbo.Advisors");
        }
    }
}
