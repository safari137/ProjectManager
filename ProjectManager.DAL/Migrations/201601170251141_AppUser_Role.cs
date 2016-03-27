namespace ProjectManager.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppUser_Role : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "Role", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "Role");
        }
    }
}
