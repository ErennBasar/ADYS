namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncModelChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CourseSelections", "IsApprovedByAdvisor", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CourseSelections", "IsApprovedByAdvisor", c => c.Boolean(nullable: false));
        }
    }
}
