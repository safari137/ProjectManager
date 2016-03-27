namespace ProjectManager.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnitId : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TimeSheetLineUnits");
            DropColumn("dbo.TimeSheetLineUnits", "Id");
            AddColumn("dbo.TimeSheetLineUnits", "UnitId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TimeSheetLineUnits", "UnitId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TimeSheetLineUnits", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.TimeSheetLineUnits");
            DropColumn("dbo.TimeSheetLineUnits", "UnitId");
            AddPrimaryKey("dbo.TimeSheetLineUnits", "Id");
        }
    }
}
