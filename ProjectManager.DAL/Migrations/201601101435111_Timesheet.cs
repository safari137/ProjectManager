namespace ProjectManager.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Timesheet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        XeroEmployeeId = c.Guid(nullable: false),
                        FullName = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Timesheets",
                c => new
                    {
                        TimeSheetId = c.Int(nullable: false, identity: true),
                        Id = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TimeSheetStatus = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TimeSheetId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.TimesheetLines",
                c => new
                    {
                        TimeSheetLineId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Guid(nullable: false),
                        CustomerName = c.String(),
                        EarningsId = c.Guid(nullable: false),
                        TimeSheetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TimeSheetLineId)
                .ForeignKey("dbo.Timesheets", t => t.TimeSheetId, cascadeDelete: true)
                .Index(t => t.TimeSheetId);
            
            CreateTable(
                "dbo.TimeSheetLineUnits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Hours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Index = c.Int(nullable: false),
                        TimeSheetLineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TimesheetLines", t => t.TimeSheetLineId, cascadeDelete: true)
                .Index(t => t.TimeSheetLineId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeSheetLineUnits", "TimeSheetLineId", "dbo.TimesheetLines");
            DropForeignKey("dbo.TimesheetLines", "TimeSheetId", "dbo.Timesheets");
            DropForeignKey("dbo.Timesheets", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.TimeSheetLineUnits", new[] { "TimeSheetLineId" });
            DropIndex("dbo.TimesheetLines", new[] { "TimeSheetId" });
            DropIndex("dbo.Timesheets", new[] { "EmployeeId" });
            DropTable("dbo.TimeSheetLineUnits");
            DropTable("dbo.TimesheetLines");
            DropTable("dbo.Timesheets");
            DropTable("dbo.Employees");
        }
    }
}
