using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Services.XeroService;

namespace ProjectManager.WebUI.Controllers
{
    [Authorize(Roles="admin")]
    public class StatusController : Controller
    {
        // GET: Status
        public ActionResult Index()
        {
            ViewBag.ApiCalls = XeroApiService.ApiCalls;
            ViewBag.PayrollCalls = XeroApiService.PayrollCalls;
            ViewBag.AccountingCalls = XeroApiService.AccountingCalls;

            return View();
        }
    }
}