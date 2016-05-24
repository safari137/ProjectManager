using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models;
using ProjectManager.Services;
using ProjectManager.Services.XeroService.Payroll;
using ProjectManager.WebUI.Infrastructure;

namespace ProjectManger.WebUI.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("Memo")]
    public class MemoController : Controller
    {
        private readonly MemoService _memoService = new MemoService(new MemoRepository(new DataContext()));
        // GET: Memo
        public ActionResult Index()
        {
            var memoList = _memoService.GetAllMemos();

            return View(memoList);
        }

        [HttpGet]
        public ActionResult CreateMemo()
        {
            ViewBag.Employees = EmployeeService.GetAllEmployees();
            return View();
        }

        [HttpPost]
        public ActionResult CreateMemo(Memo memo)
        {
            _memoService.CreateMemo(memo.Notes, memo.Employee.XeroEmployeeId);

            return RedirectToAction("Index");
        }

        public ActionResult DeleteMemo(int id)
        {
            _memoService.DeleteMemo(id);

            return RedirectToAction("Index");
        }
    }
}