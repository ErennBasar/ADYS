namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdvisorAssignmentTable : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvisorAssignments", "StudentId", "dbo.Users");
            DropForeignKey("dbo.AdvisorAssignments", "AdvisorId", "dbo.Users");
            DropIndex("dbo.AdvisorAssignments", new[] { "AdvisorId" });
            DropIndex("dbo.AdvisorAssignments", new[] { "StudentId" });
            DropTable("dbo.AdvisorAssignments");
        }
    }
}
