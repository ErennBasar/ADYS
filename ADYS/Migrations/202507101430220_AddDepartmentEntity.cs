namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepartmentEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            AddColumn("dbo.Courses", "Kontenjan", c => c.Int(nullable: false));
            AddColumn("dbo.Courses", "AdvisorId", c => c.Int(nullable: false));
            AddColumn("dbo.Courses", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.Students", "FullName", c => c.String(nullable: false));
            AlterColumn("dbo.Students", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Students", "Password", c => c.String(nullable: false));
            CreateIndex("dbo.Courses", "AdvisorId");
            CreateIndex("dbo.Courses", "DepartmentId");
            AddForeignKey("dbo.Courses", "AdvisorId", "dbo.Advisors", "AdvisorId");
            AddForeignKey("dbo.Courses", "DepartmentId", "dbo.Departments", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Courses", "AdvisorId", "dbo.Advisors");
            DropIndex("dbo.Courses", new[] { "DepartmentId" });
            DropIndex("dbo.Courses", new[] { "AdvisorId" });
            AlterColumn("dbo.Students", "Password", c => c.String());
            AlterColumn("dbo.Students", "Email", c => c.String());
            AlterColumn("dbo.Students", "FullName", c => c.String());
            DropColumn("dbo.Courses", "DepartmentId");
            DropColumn("dbo.Courses", "AdvisorId");
            DropColumn("dbo.Courses", "Kontenjan");
            DropTable("dbo.Departments");
        }
    }
}
