using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models.Xero;
using Xero.Api.Payroll.Australia.Model.Status;
using Xero.Api.Payroll.Common.Model;
using ProjectManagerTimeSheet = ProjectManager.Models.Xero.Timesheet;
using Timesheet = Xero.Api.Payroll.America.Model.Timesheet;
using XeroTimesheetLine = Xero.Api.Payroll.America.Model.TimesheetLine;

namespace ProjectManager.Services.XeroService.Payroll
{
    public class TimeSheetService
    {
        private readonly TimesheetRepository _timesheetRepository = new TimesheetRepository(new DataContext());

        public List<Xero.Api.Payroll.America.Model.Timesheet> GetTimeSheets(DateTime beginDate, DateTime endDate)
        {
            var timesheets = XeroApiService.PayrollApi.Timesheets
                .Find()
                .Where(ts => (ts.StartDate >= beginDate) && (ts.EndDate <= endDate))
                .OrderBy(ts => ts.EndDate)
                .ToList();

            return timesheets;
        }

        public List<Xero.Api.Payroll.America.Model.Timesheet> GetTimeSheets()
        {
            var timesheets = XeroApiService.PayrollApi.Timesheets
                .Find()
                .OrderBy(ts => ts.EndDate)
                .Where(ts => ts.Status == TimesheetStatus.Processed)
                .ToList();

            return timesheets;
        }

        public Xero.Api.Payroll.America.Model.Timesheet GetTimesheetById(Guid id)
        {
            var timesheet = XeroApiService.PayrollApi
                .Timesheets
                .Find()
                .FirstOrDefault(ts => ts.Id == id);

            return timesheet;
        }

        public void PublishToXero(ProjectManagerTimeSheet timesheet)
        {
            MarkTimesheetProcessed(timesheet.TimeSheetId);

            var xeroTimesheet = ConvertToXeroTimeSheet(timesheet);

            xeroTimesheet = ConvertTimeSheetLinesToXero(timesheet, xeroTimesheet);

            XeroApiService.PayrollApi
                .Create(xeroTimesheet);
        }

        private void MarkTimesheetProcessed(int id)
        {
            var timesheet = _timesheetRepository.GetById(id);

            if (timesheet == null)
                return;

            timesheet.TimeSheetStatus = TimeSheetStatus.Processed;

            _timesheetRepository.Update(timesheet);
            _timesheetRepository.Commit();
        }

        private static Timesheet ConvertTimeSheetLinesToXero(ProjectManagerTimeSheet timesheet, Timesheet xeroTimesheet)
        {
            foreach (var line in timesheet.TimesheetLines)
            {
                var timesheetLine = new Xero.Api.Payroll.America.Model.TimesheetLine
                {
                    TrackingItemID = line.CustomerId,
                    EarningsTypeId = new Guid("279425f0-3b66-45b1-afb3-b8c5161b786c"),
                    NumberOfUnits = new NumberOfUnits()
                };

                foreach (var unit in line.Units.OrderBy(u => u.Index))
                    timesheetLine.NumberOfUnits.Add(unit.Hours);

                xeroTimesheet.TimesheetLines.Add(timesheetLine);
            }

            return xeroTimesheet;
        }

        private static Timesheet ConvertToXeroTimeSheet(ProjectManagerTimeSheet timesheet)
        {
            var xeroTimesheet = new Xero.Api.Payroll.America.Model.Timesheet()
            {
                EmployeeId = timesheet.Employee.XeroEmployeeId,
                EndDate = timesheet.EndDate,
                StartDate = timesheet.StartDate,
                Status = TimesheetStatus.Draft,
                TimesheetLines = new List<XeroTimesheetLine>()
            };
            return xeroTimesheet;
        }
    }
}
