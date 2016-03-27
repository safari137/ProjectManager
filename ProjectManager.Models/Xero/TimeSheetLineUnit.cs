using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models.Xero
{
    public class TimeSheetLineUnit
    {
        [Key]
        public int UnitId { get; set; }

        public decimal Hours { get; set; }

        [DisplayName("Notes")]
        public string Description { get; set; }

        public int Index { get; set; }

        public int TimeSheetLineId { get; set; }

        public virtual TimesheetLine TimesheetLine { get; set; }

    }
}
