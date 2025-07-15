namespace ADYS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTermTable : DbMigration
    {
        public override void Up()
        {
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
            DropTable("dbo.Terms");
        }
    }
}
