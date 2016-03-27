using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models.Xero;

namespace ProjectManager.Services.XeroService.Payroll
{
    public class TimesheetModelUpdater
    {
        private readonly IRepository<Timesheet> _timesheetRepository = new TimesheetRepository(new DataContext()); 
        private readonly IRepository<TimesheetLine> _timesheetLineRepository = new TimesheetLineRepository(new DataContext());
        private readonly IRepository<TimeSheetLineUnit> _timesheetLineUnitRepository = new TimesheetLineUnitRepository(new DataContext());

        public TimesheetModelUpdater()
        {
        }

        public void AddLine(int timesheetId, TimesheetLine timesheetLine)
        {
            timesheetLine.TimeSheetId = timesheetId;
            _timesheetLineRepository.Insert(timesheetLine);
            _timesheetLineRepository.Commit();
        }

        public void AddUnit(int timesheetLineId, TimeSheetLineUnit timesheetLineUnit)
        {
            timesheetLineUnit.TimeSheetLineId = timesheetLineId;
            _timesheetLineUnitRepository.Insert(timesheetLineUnit);
        }

        public void UpdateAll(Timesheet timesheet)
        {
            foreach (var line in timesheet.TimesheetLines)
            {
                foreach (var unit in line.Units)
                {
                    UpdateUnit(unit);
                }

                UpdateLine(line);
            }
            _timesheetRepository.Update(timesheet);
        }

        private void UpdateLine(TimesheetLine line)
        {
                _timesheetLineRepository.Update(line);
        }

        private void UpdateUnit(TimeSheetLineUnit unit)
        {
            _timesheetLineUnitRepository.Update(unit);
        }

        public int Commit()
        {
            var totalCommit = 0;

            totalCommit += _timesheetLineUnitRepository.Commit();

            return totalCommit;
        }
    }
}
