using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models.Xero;
using ProjectManager.Services.XeroService;
using ProjectManager.WebUI.Infrastructure;

namespace ProjectManager.WebUI.Controllers
{
    [Authorize(Roles="manager, admin")]
    [SelectedTab("Tracking")]
    public class TrackingController : Controller
    {
        private readonly TrackingService _trackingService = new TrackingService();
        
        public ActionResult Index()
        {
            var items = _trackingService.GetTrackingItems();

            return View(items);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TrackingItem trackingItem)
        {
            if (!ModelState.IsValid)
                return View(trackingItem);

            _trackingService.Insert(trackingItem);

            return RedirectToAction("Index");
        }
    }
}