using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using ProjectManager.Models;
using ProjectManager.Models.Xero;

namespace ProjectManager.DAL.Data
{
    public class DataContext : DbContext
    {
        public DbSet<ItemGroup> ItemGroups { get; set; }

        public DbSet<XeroItemCode> XeroItemCodes { get; set; }

        public DbSet<Timesheet> Timesheets { get; set; }

        public DbSet<TimesheetLine> TimesheetLines { get; set; }

        public DbSet<TimeSheetLineUnit> TimeSheetLineUnits { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<AppUser> Users { get; set; }

        public DbSet<Memo> Memos { get; set; }
    }
}
