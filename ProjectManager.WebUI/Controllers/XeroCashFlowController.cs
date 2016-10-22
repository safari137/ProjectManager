using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.WebUI.Infrastructure;

namespace ProjectManager.WebUI.Controllers
{
    [Authorize(Roles="viewer, manager, admin")]
    [SelectedTab("XeroCashFlow")]
    public class XeroCashFlowController : Controller
    {
        private readonly IXeroCashflowConnection _xeroConnection;

        public XeroCashFlowController()
        {
            
        }

        public ActionResult Index()
        {

            return View();
        }
    }
}