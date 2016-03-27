using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models.Xero
{
    public class TimesheetLine
    {
        [Key]
        public int TimeSheetLineId { get; set; }
         
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }

        public Guid EarningsId { get; set; }

        public virtual List<TimeSheetLineUnit> Units  { get; set; }

        public int TimeSheetId { get; set; }

        public virtual Timesheet Timesheet { get; set; }
    }
}
