namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCourseSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "DayOfWeek", c => c.String());
            AddColumn("dbo.Courses", "StartTime", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Courses", "EndTime", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "EndTime");
            DropColumn("dbo.Courses", "StartTime");
            DropColumn("dbo.Courses", "DayOfWeek");
        }
    }
}
