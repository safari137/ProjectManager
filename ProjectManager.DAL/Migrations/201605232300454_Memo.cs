namespace ProjectManager.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Memo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Memos",
                c => new
                    {
                        MemoId = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        Notes = c.String(),
                        EmployeeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MemoId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Memos", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Memos", new[] { "EmployeeId" });
            DropTable("dbo.Memos");
        }
    }
}
