namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModels : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "AdvisorId");
            RenameColumn(table: "dbo.Users", name: "Advisor_UserId", newName: "AdvisorId");
            RenameIndex(table: "dbo.Users", name: "IX_Advisor_UserId", newName: "IX_AdvisorId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Users", name: "IX_AdvisorId", newName: "IX_Advisor_UserId");
            RenameColumn(table: "dbo.Users", name: "AdvisorId", newName: "Advisor_UserId");
            AddColumn("dbo.Users", "AdvisorId", c => c.Int());
        }
    }
}
