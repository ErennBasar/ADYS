namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Courses", "User_UserId", "dbo.Users");
            DropIndex("dbo.Courses", new[] { "User_UserId" });
            DropColumn("dbo.Courses", "User_UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "User_UserId", c => c.Int());
            CreateIndex("dbo.Courses", "User_UserId");
            AddForeignKey("dbo.Courses", "User_UserId", "dbo.Users", "UserId");
        }
    }
}
