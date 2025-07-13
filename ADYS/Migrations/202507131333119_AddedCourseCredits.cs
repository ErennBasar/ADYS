namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCourseCredits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseSelections", "AdvisorFullName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseSelections", "AdvisorFullName");
        }
    }
}
