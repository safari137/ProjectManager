using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.Services.XeroService.TransactionLoaders;
using ProjectManager.Services.XeroService.TransactionLoaders.StatementImporters;
using ProjectManager.WebUI.Infrastructure;

namespace ProjectManager.WebUI.Controllers
{
    [Authorize(Roles="viewer, manager, admin")]
    [SelectedTab("XeroCashFlow")]
    public class XeroCashFlowController : Controller
    {
        private readonly IXeroCashflowConnection _xeroConnection;
        private readonly DateTime _startTime;

        public Dictionary<int, string> CalendarMonth = new Dictionary<int, string>()
        {
            { 1, "January"},
            { 2, "February" },
            { 3, "March" },
            { 4, "April" },
            { 5, "May" },
            { 6, "June"},
            { 7, "July" },
            { 8, "August" },
            { 9, "September" },
            {10, "October" },
            {11, "November" },
            {12, "December" }
        };

        public XeroCashFlowController(IXeroCashflowConnection xeroConnection)
        {
            this._startTime = DateTime.Now;
            this._xeroConnection = xeroConnection;
            this.RegisterTransactionLoaders();
        }

        public ActionResult Index()
        {
            var orderedTransactions = _xeroConnection.GetOrderedTransactions();

            ViewBag.BeginningBalance = _xeroConnection.GetBalance();

            ViewBag.ElapsedTime = DateTime.Now - _startTime;

            return View(orderedTransactions);
        }

        public ActionResult IndexCalendar()
        {
            ViewBag.CalendarMonth = CalendarMonth;

            return View();

        }

        private void RegisterTransactionLoaders()
        {
            _xeroConnection.RegisterTransactionLoader(new RepeatingInvoiceTransactionLoader(20, 4));
            _xeroConnection.RegisterTransactionLoader(new InvoiceAndBillsLoader());
            _xeroConnection.RegisterTransactionLoader(new BbtCcStatementImporter());
            _xeroConnection.RegisterTransactionLoader(new LowesStatementImporter());
            _xeroConnection.RegisterTransactionLoader(new PayrollBillTrasactionLoader(new Guid("3a58979f-5752-4ef2-b190-19518ed6908f"), "Internal Revenue Service"));
            _xeroConnection.RegisterTransactionLoader(new PayrollBillTrasactionLoader(new Guid("2904b4e2-2551-4c92-a6fc-8912117a3a60"), "Virginia Department Of Taxation"));
            _xeroConnection.RegisterTransactionLoader(new PayrollBillTrasactionLoader(new Guid("db835228-bdfb-48b7-98ac-e9a445b90be4"), "Virginia Employment Commission"));
        }
    }
}