using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models.Xero
{
    public class Timesheet
    {
        [Key]
        public int TimeSheetId { get; set; }

        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        
        public TimeSheetStatus TimeSheetStatus { get; set; }

        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual List<TimesheetLine> TimesheetLines { get; set; }
    }

    public enum TimeSheetStatus
    {
        Draft,
        Approved,
        Processed,
    };
}
