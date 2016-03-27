using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.Models;
using ProjectManager.Services;
using ProjectManager.WebUI.Models;

namespace ProjectManger.WebUI.Controllers
{
    public class CashflowController : Controller
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IBalanceService _balanceService;

        public CashflowController(IRepository<Transaction> transactionRepository)
        {
            this._transactionRepository = transactionRepository;
            _balanceService = new CashFlowBalanceService(new BalanceFileService());
        }
 
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            _balanceService.RetrieveBalance();

            ViewBag.Balance = CashFlowBalance.Balance;
            ViewBag.LastUpdated = CashFlowBalance.LastUpdated;

            var transactions = _transactionRepository
                .GetAll()
                .OrderBy(t => t.Date)
                .ThenBy(t => t.TransactionType);

            return View(transactions);
        }

        [HttpGet]
        public ActionResult UpdateBalance()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateBalance(CashFlowBalanceModelView updatedModel)
        {
           _balanceService.UpdateBalance(updatedModel.Balance, DateTime.Now);

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult CommitTransaction(int id)
        {
            var transaction = _transactionRepository.GetById(id);

            ViewBag.SelectList = new SelectList(new List<Job> {transaction.Job}, "JobId", "SelectListName");

            return View(transaction);
        }

        [HttpPost]
        public ActionResult CommitTransaction(Transaction transaction)
        {
            UpdateCommittedBalance(transaction);

            _transactionRepository.Delete(transaction.TransactionId);
            _transactionRepository.Commit();

            return RedirectToAction("List");
        }

        private void UpdateCommittedBalance(Transaction transaction)
        {
            var newBalance = (transaction.TransactionType == TransactionType.Deposit)
                ? CashFlowBalance.Balance + transaction.Amount
                : CashFlowBalance.Balance - transaction.Amount;

            _balanceService.UpdateBalance(newBalance, DateTime.Now);
        }
    }
}