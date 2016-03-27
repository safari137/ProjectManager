using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;
using ProjectManager.Models.Xero;

namespace ProjectManager.DAL.Repositories
{
    public class TimesheetRepository : RepositoryBase<Timesheet>
    {
        public TimesheetRepository(DataContext context) : base(context)
        {
            
        }

        public void UpdateEntireEntity(Timesheet timesheet)
        {
            var existingSheet = Context.Timesheets
                .Where(p => p.TimeSheetId == timesheet.TimeSheetId)
                .Include(p => p.TimesheetLines)
                .SingleOrDefault();

            if (existingSheet == null) return;
            // Update parent
            Context.Entry(existingSheet).CurrentValues.SetValues(timesheet);

            // Delete children
            foreach (var existingLine in existingSheet.TimesheetLines.ToList())
            {
                if (!timesheet.TimesheetLines.Any(c => c.TimeSheetId == existingLine.TimeSheetId))
                    Context.TimesheetLines.Remove(existingLine);
            }

            // Update and Insert children
            foreach (var lineModel in timesheet.TimesheetLines)
            {
                var existingLine = existingSheet.TimesheetLines
                    .SingleOrDefault(c => c.TimeSheetLineId == lineModel.TimeSheetLineId);

                if (existingLine != null)
                    Context.Entry(existingLine).CurrentValues.SetValues(lineModel);
                else
                {
                    // Insert child
                    var newLine = new TimesheetLine
                    {
                        CustomerId = lineModel.CustomerId,
                        EarningsId = lineModel.EarningsId,
                        TimeSheetId = lineModel.TimeSheetId
                    };
                    existingSheet.TimesheetLines.Add(newLine);
                }

                foreach (var existingUnit in existingLine.Units)
                {
                    if (!lineModel.Units.Any(c => c.TimeSheetLineId == existingUnit.TimeSheetLineId))
                        Context.TimeSheetLineUnits.Remove(existingUnit);
                }

                foreach (var unitModel in lineModel.Units)
                {
                    var existingUnit = existingLine?.Units
                        .SingleOrDefault(c => c.UnitId == unitModel.UnitId);

                    if (existingUnit != null)
                        Context.Entry(existingUnit).CurrentValues.SetValues(unitModel);
                    else
                    {
                        var newUnit = new TimeSheetLineUnit
                        {
                            Description = unitModel.Description,
                            Hours = unitModel.Hours,
                            Index = unitModel.Index,
                            TimeSheetLineId = unitModel.TimeSheetLineId
                        };
                        existingLine.Units.Add(newUnit);
                    }
                }
            }

            Context.SaveChanges();
        }
    }
}
