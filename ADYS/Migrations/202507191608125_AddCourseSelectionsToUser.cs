namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCourseSelectionsToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "User_UserId", c => c.Int());
            AddColumn("dbo.Users", "AdvisorId", c => c.Int());
            AddColumn("dbo.Users", "Advisor_UserId", c => c.Int());
            AddColumn("dbo.CourseSelections", "User_UserId", c => c.Int());
            CreateIndex("dbo.Courses", "User_UserId");
            CreateIndex("dbo.Users", "Advisor_UserId");
            CreateIndex("dbo.CourseSelections", "User_UserId");
            AddForeignKey("dbo.Users", "Advisor_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Courses", "User_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.CourseSelections", "User_UserId", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseSelections", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Courses", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "Advisor_UserId", "dbo.Users");
            DropIndex("dbo.CourseSelections", new[] { "User_UserId" });
            DropIndex("dbo.Users", new[] { "Advisor_UserId" });
            DropIndex("dbo.Courses", new[] { "User_UserId" });
            DropColumn("dbo.CourseSelections", "User_UserId");
            DropColumn("dbo.Users", "Advisor_UserId");
            DropColumn("dbo.Users", "AdvisorId");
            DropColumn("dbo.Courses", "User_UserId");
        }
    }
}
