namespace ProjectManager.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppUserEmployeeRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "Employee_EmployeeId", c => c.Int());
            CreateIndex("dbo.AppUsers", "Employee_EmployeeId");
            AddForeignKey("dbo.AppUsers", "Employee_EmployeeId", "dbo.Employees", "EmployeeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppUsers", "Employee_EmployeeId", "dbo.Employees");
            DropIndex("dbo.AppUsers", new[] { "Employee_EmployeeId" });
            DropColumn("dbo.AppUsers", "Employee_EmployeeId");
        }
    }
}
