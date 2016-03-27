using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models;
using ProjectManager.Models.Xero;
using ProjectManager.Services.XeroService;
using ProjectManager.Services.XeroService.Payroll;
using ProjectManager.WebUI.Models;
using ProjectManager.WebUI.Infrastructure;

namespace ProjectManager.WebUI.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("Timesheets")]
    public class PayrollController : Controller
    {
        private readonly TimeSheetService _xeroTimeSheetService = new TimeSheetService();
        private readonly TimesheetModelUpdater _timesheetModelUpdater = new TimesheetModelUpdater();
        private readonly TimesheetRepository _timesheetRepository = new TimesheetRepository(new DataContext());
        private readonly IRepository<Employee> _employeeRepository = new EmployeeRepository(new DataContext());

        public PayrollController()
        {
            
        }

        public ActionResult Index()
        {
            return RedirectToAction("TimesheetList");
        }

        public ActionResult TimesheetList()
        {
            var timesheets = _timesheetRepository.GetAll()
                .OrderBy(ts => ts.TimeSheetStatus)
                .ThenBy((ts => ts.EndDate));

            return View(timesheets);
        }

        public ActionResult TimeSheetDetails(int id)
        {
            var timesheet = _timesheetRepository.GetById(id);

            ViewBag.CustomerDictionary = XeroTrackingCategoryConnection.GetAll();
            
            return View(timesheet);
        }

        [HttpGet]
        public ActionResult CreateTimeSheet()
        {
            ViewBag.Employees = EmployeeService.GetAllEmployees();

            var lastDate = _xeroTimeSheetService.GetTimeSheets().LastOrDefault()?.EndDate;

            ViewBag.EndDates = new List<DateTime>
            {
                lastDate.Value.AddDays(7),
                lastDate.Value.AddDays(14),
                lastDate.Value.AddDays(21)
            };

            return View();
        }

        [HttpPost]
        public ActionResult CreateTimeSheet(CreateTimeSheetViewModel createTimeSheetViewModel)
        {
            var employeeRecord = _employeeRepository
                .GetAll()
                .SingleOrDefault(e => e.XeroEmployeeId == createTimeSheetViewModel.XeroEmployeeId);

            if (employeeRecord == null)
                throw new InvalidOperationException("employee not found");

            var timesheet = new Timesheet
            {
                EmployeeId = employeeRecord.EmployeeId,
                StartDate = createTimeSheetViewModel.EndDate.AddDays(-6),
                EndDate = createTimeSheetViewModel.EndDate,
                TimesheetLines = new List<TimesheetLine>(),
                TimeSheetStatus = TimeSheetStatus.Draft
            };
            timesheet.TimesheetLines.Add(GetNewLine());

            _timesheetRepository.Insert(timesheet);
            _timesheetRepository.Commit();

            return RedirectToAction("TimeSheetEdit", new { id = timesheet.TimeSheetId });
        }

        [HttpGet]
        public ActionResult TimeSheetEdit(int id)
        {
            var timesheet = _timesheetRepository.GetById(id);

            ViewBag.Customers = XeroTrackingCategoryConnection.GetAll().ToList();

            return View(timesheet);
        }

        [HttpPost]
        public ActionResult TimeSheetEdit(Timesheet timesheet, string submitType)
        {
            switch (submitType)
            {
                case "AddLine":
                    ViewBag.Customers = XeroTrackingCategoryConnection.GetAll().ToList();

                    this.CreateTimeSheetLine(timesheet.TimeSheetId);

                    _timesheetRepository.UpdateEntireEntity(timesheet);

                    return RedirectToAction("TimeSheetEdit", new {id=timesheet.TimeSheetId});

                case "Submit":
                    _timesheetRepository.UpdateEntireEntity(timesheet);

                    ViewBag.Changes = _timesheetRepository.Commit();

                    break;
            }
            return View("TimeSheetDetails", timesheet);
        }

        private static TimesheetLine GetNewLine()
        {
            var timesheetLine = new TimesheetLine {Units = new List<TimeSheetLineUnit>()};

            for (var i = 0; i < 7; i++)
            {
                timesheetLine.Units.Add(new TimeSheetLineUnit
                {
                    Hours = 0,
                    Description = "",
                    Index = i
                });
            }

            return timesheetLine;
        }

        private void CreateTimeSheetLine(int timesheetId)
        {
            var timesheetLine = GetNewLine();
            _timesheetModelUpdater.AddLine(timesheetId, timesheetLine);
        }
        
        public ActionResult TimeSheetDelete(int id)
        {
            _timesheetRepository.Delete(id);
            _timesheetRepository.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult ViewEmployees()
        {
            var employees = _employeeRepository.GetAll();

            return View(employees);
        }

        public ActionResult PublishToXero(int id)
        {
            var timesheet = _timesheetRepository.GetById(id);

            _xeroTimeSheetService.PublishToXero(timesheet);

            return RedirectToAction("Index");
        }
    }
}