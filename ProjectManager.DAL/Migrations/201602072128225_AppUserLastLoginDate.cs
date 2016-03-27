namespace ProjectManager.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppUserLastLoginDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "LastLoginDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "LastLoginDate");
        }
    }
}
