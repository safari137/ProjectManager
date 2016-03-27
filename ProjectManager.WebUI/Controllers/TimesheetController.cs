using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models;
using ProjectManager.Services.XeroService;
using ProjectManager.Services.XeroService.Payroll;
using ProjectManager.WebUI.Infrastructure;

namespace ProjectManager.WebUI.Controllers
{
    [Authorize(Roles = "manager, admin")]
    [SelectedTab("TimeEntry")]
    public class TimesheetController : Controller
    {
        private readonly SingleDayEntryService _singleDayEntryService;
        private readonly TimesheetRepository _timesheetRepository = new TimesheetRepository(new DataContext());

        public TimesheetController()
        {
            _singleDayEntryService = new SingleDayEntryService(_timesheetRepository, new AppUserRepository(new DataContext()));
        }

        public ActionResult Index()
        {
            return RedirectToAction("CreateTimesheetEntry");
        }

        [HttpGet]
        public ActionResult CreateTimesheetEntry()
        {
            var today = DateTime.Today;

            var user = User.Identity.GetUserName();

            var timesheet = _singleDayEntryService.GetSheet(user, today);

            ViewBag.TimeSheetId = timesheet.TimeSheetId;
            ViewBag.Customers = XeroTrackingCategoryConnection.GetAll().ToList();

            var timesheetEntryList = new List<SingleDayTimeEntry> { new SingleDayTimeEntry() };

            return View(timesheetEntryList);
        }
     
        [HttpPost]
        public ActionResult CreateTimesheetEntry(List<SingleDayTimeEntry> singleDayTimeEntries, int timesheetId, string submitType)
        {
            if (!ModelState.IsValid)
                return View(singleDayTimeEntries);

            ViewBag.TimeSheetId = timesheetId;
            ViewBag.Customers = XeroTrackingCategoryConnection.GetAll().ToList();

            switch (submitType)
            {
                case "Add Line":
                    singleDayTimeEntries.Add(new SingleDayTimeEntry());
                    return View(singleDayTimeEntries);
                case "Submit":
                    _singleDayEntryService.Save(singleDayTimeEntries, timesheetId);
                    break;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}