namespace ProjectManager.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemGroups",
                c => new
                    {
                        ItemGroupId = c.Int(nullable: false, identity: true),
                        ItemGroupName = c.String(),
                        XeroGroupItemCode = c.String(),
                    })
                .PrimaryKey(t => t.ItemGroupId);
            
            CreateTable(
                "dbo.XeroItemCodes",
                c => new
                    {
                        XeroItemCodeId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.XeroItemCodeId)
                .ForeignKey("dbo.ItemGroups", t => t.ItemGroupId, cascadeDelete: true)
                .Index(t => t.ItemGroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.XeroItemCodes", "ItemGroupId", "dbo.ItemGroups");
            DropIndex("dbo.XeroItemCodes", new[] { "ItemGroupId" });
            DropTable("dbo.XeroItemCodes");
            DropTable("dbo.ItemGroups");
        }
    }
}
