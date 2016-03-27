using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models;
using ProjectManager.Models.Xero;

namespace ProjectManager.Services.XeroService.Payroll
{
    public class SingleDayEntryService
    {
        private readonly TimesheetRepository _timesheetRepository;
        private readonly TimesheetLineRepository _timesheetLineRepository = new TimesheetLineRepository(new DataContext());
        private readonly TimesheetLineUnitRepository _timesheetLineUnitRepository = new TimesheetLineUnitRepository(new DataContext());
        private readonly IRepository<AppUser> _appUserRepository;
        private readonly TimeSheetService _timesheetService = new TimeSheetService();

        public SingleDayEntryService(TimesheetRepository timesheetRepository, IRepository<AppUser> appUseRepository)
        {
            this._timesheetRepository = timesheetRepository;
            this._appUserRepository = appUseRepository;
        }

        public Timesheet GetSheet(string appUserName, DateTime date)
        {
            var appUser = _appUserRepository.GetAll()
                .SingleOrDefault(a => a.Username == appUserName);

            if (appUser == null)
                return null;

            var timesheet = GetOrCreateTimeSheet(date, appUser);

            var dayNumeric = GetDayOfWeekNumeric(date.DayOfWeek);

            return timesheet.TimesheetLines
                .Where(line => line.Units.Count > dayNumeric+1)
                .Any(line => line.Units[dayNumeric].Hours > 0) ? null : timesheet;
        }

        private Timesheet GetOrCreateTimeSheet(DateTime date, AppUser appUser)
        {
            var timesheet = _timesheetRepository.GetAll()
                .Where(ts => ts.StartDate <= date && ts.EndDate >= date)
                .SingleOrDefault(ts => ts.EmployeeId == appUser.Employee.EmployeeId) ??
                            CreateSheet(appUser.Employee, date);

            return timesheet;
        }

        private Timesheet CreateSheet(Employee employee, DateTime date)
        {
            var startDate = getStartDate(date);

            var timesheet = new Timesheet
            {
                EmployeeId = employee.EmployeeId,
                EndDate = startDate.AddDays(6),
                Id = employee.XeroEmployeeId,
                StartDate = startDate,
                TimesheetLines = new List<TimesheetLine>(),
                TimeSheetStatus = TimeSheetStatus.Draft
            };

            _timesheetRepository.Insert(timesheet);
            _timesheetRepository.Commit();

            return timesheet;
        }

        private static DateTime getStartDate(DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Saturday)
                date = date.AddDays(-1);
            return date;
        }

        public void Save(IEnumerable<SingleDayTimeEntry> timeEntries, int timesheetId)
        {
            var timesheetLines = ConvertSingleDayViewModelToTimesheetLines(timeEntries, timesheetId);

            var timeSheet = _timesheetRepository.GetById(timesheetId);

            if (timeSheet == null)
                throw new InvalidOperationException("No timesheet found.");

            foreach (var line in timesheetLines)
            {
                TimesheetLine existingLine;
                if ((existingLine = GetExistingLineOrDefault(line, timeSheet)) != null)
                    UpdateTimeSheetLine(line, existingLine);
                else
                {
                    _timesheetLineRepository.Insert(line);
                    _timesheetLineRepository.Commit();
                }
            }
        }

        private static TimesheetLine GetExistingLineOrDefault(TimesheetLine line, Timesheet timesheet)
        {
            return timesheet.TimesheetLines
                        .FirstOrDefault(ts => ts.CustomerId == line.CustomerId);
        }

        private void UpdateTimeSheetLine(TimesheetLine newLine, TimesheetLine existingLine)
        {
            existingLine = _timesheetLineRepository.GetById(existingLine.TimeSheetLineId);

            for (var i = 0; i < newLine.Units.Count; i++)
            {
                if (newLine.Units[i].Hours <= 0) continue;

                var idToDelete = GetCorrespondingUnitId(existingLine.Units, newLine.Units[i].Index);
                _timesheetLineUnitRepository.Delete(idToDelete);
                newLine.Units[i].TimeSheetLineId = existingLine.TimeSheetLineId;
                _timesheetLineUnitRepository.Insert(newLine.Units[i]);
            }
            _timesheetLineUnitRepository.Commit();
            _timesheetLineRepository.Commit();
        }

        private int GetCorrespondingUnitId(IEnumerable<TimeSheetLineUnit> units, int index)
        {
            var currespondingUnit = units.FirstOrDefault(u => u.Index == index);

            return currespondingUnit?.UnitId ?? -1;
        }

        private static int GetDayOfWeekNumeric(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return 1;
                case DayOfWeek.Sunday:
                    return 2; 
                case DayOfWeek.Monday:
                    return 3;
                case DayOfWeek.Tuesday:
                    return 4;
                case DayOfWeek.Wednesday:
                    return 5;
                case DayOfWeek.Thursday:
                    return 6;
                case DayOfWeek.Friday:
                    return 7;
                default:
                    return -1;
            }
        }

        private static List<TimesheetLine> ConvertSingleDayViewModelToTimesheetLines(IEnumerable<SingleDayTimeEntry> timeEntries, int timesheetId)
        {
            var timesheetLines = new List<TimesheetLine>();

            foreach (var entry in timeEntries)
            {
                var timesheetLine = new TimesheetLine
                {
                    CustomerId = entry.CustomerId,
                    Units = new List<TimeSheetLineUnit>(),
                    TimeSheetId = timesheetId
                };

                for(var i=0; i<7;i++)
                    timesheetLine.Units.Add(new TimeSheetLineUnit { Index = i });

                var unitIndex = GetDayOfWeekNumeric(DateTime.Today.DayOfWeek) - 1;

                var unit = timesheetLine.Units.FirstOrDefault(u => u.Index == unitIndex);

                if (unit == null)
                    throw new NullReferenceException("unit is null");

                unit.Description = entry.Notes;
                unit.Hours = entry.Hours;
                unit.Index = unitIndex;

                timesheetLines.Add(timesheetLine);
            }

            return timesheetLines;
        }
    }
}
