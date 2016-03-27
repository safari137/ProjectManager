using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.Models;

namespace ProjectManger.WebUI.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IRecurringTransaction _recurringTransactionService;
        private readonly IRepository<RecurringTransaction> _recurringTransactionRepository; 

        public TransactionController(IRepository<Transaction> transactionRepository, IRepository<Job> jobRepository, 
            IRecurringTransaction recurringTransactionService, IRepository<RecurringTransaction> recurringTransactionRepository)
        {
            this._transactionRepository = transactionRepository;
            this._jobRepository = jobRepository;
            this._recurringTransactionService = recurringTransactionService;
            this._recurringTransactionRepository = recurringTransactionRepository;
        }

        public ActionResult Index()
        {
            var transactions = _transactionRepository
                .GetAll()
                .OrderBy(transaction => transaction.Date)
                .ThenBy(transaction => transaction.TransactionType);

            return View(transactions);
        }

        [HttpGet]
        public ActionResult CreateTransactionFromScratch()
        {
            var jobs = GetAllJobs();

            ViewBag.Jobs = jobs;
            ViewBag.ReturnUrl = Request.UrlReferrer;

            return View("CreateTransaction");
        }

        [HttpPost]
        public ActionResult CreateTransactionFromScratch(Transaction transaction, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                _transactionRepository.Insert(transaction);
                _transactionRepository.Commit();

                return Redirect(returnUrl);
            }

            ViewBag.Jobs = GetAllJobs();

            ViewBag.ReturnUrl = returnUrl;

            return View("CreateTransaction", transaction);
        }

        private IOrderedEnumerable<Job> GetAllJobs()
        {
            return _jobRepository
                .GetAll()
                .OrderBy(job => job.Project.Customer.Name)
                .ThenBy(job => job.Project.Name)
                .ThenBy(job => job.Name);
        }

        [HttpGet]
        public ActionResult CreateTransaction(int id)
        {
            ViewBag.Jobs = new List<Job> {_jobRepository.GetById(id)};

            return View();
        }

        [HttpPost]
        public ActionResult CreateTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _transactionRepository.Insert(transaction);
                _transactionRepository.Commit();

                return RedirectToAction("JobDetails", "Job", new {id = transaction.JobId});
            }
            ViewBag.Jobs = new List<Job> {_jobRepository.GetById(transaction.JobId)};

            return View(transaction);
        }

        public ActionResult TransactionDetails(int id)
        {
            var transaction = _transactionRepository.GetById(id);

            return View(transaction);
        }

        public ActionResult DeleteTransaction(int id)
        {
            _transactionRepository.Delete(id);
            _transactionRepository.Commit();

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpGet]
        public ActionResult EditTransaction(int id)
        {
            var transaction = _transactionRepository.GetById(id);

            ViewBag.Jobs = new List<Job> {transaction.Job};
            ViewBag.ReturnUrl = Request.UrlReferrer;

            return View(transaction);
        }

        [HttpPost]
        public ActionResult EditTransaction(Transaction transaction, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                _transactionRepository.Update(transaction);
                _transactionRepository.Commit();

                return Redirect(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(transaction);
        }

        [HttpGet]
        public ActionResult CloneTransaction(int id)
        {
            var transaction = _transactionRepository.GetById(id);

            var jobs = new List<Job> {transaction.Job};

            ViewBag.Jobs = new SelectList(jobs, "JobId", "SelectListName");

            return View(transaction);
        }

        [HttpPost]
        public ActionResult CloneTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _transactionRepository.Insert(transaction);
                _transactionRepository.Commit();

                return RedirectToAction("List", "Cashflow");
            }

            return View(transaction);
        }

        public ActionResult RecurringTransactionList()
        {
            var recurringTransactions = _recurringTransactionRepository
                .GetAll()
                .OrderBy(r => r.StartingDate);
            
            return View(recurringTransactions);
        }

        [HttpGet]
        public ActionResult CreateRecurringTransaction()
        {
            ViewBag.Jobs = _jobRepository
                .GetAll()
                .OrderBy(j => j.Name);

            return View();
        }

        [HttpPost]
        public ActionResult CreateRecurringTransaction(RecurringTransactionModels recurringTransaction)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Jobs = _jobRepository
                    .GetAll()
                    .OrderBy(j => j.Name);

                return View(recurringTransaction);
            }

            _recurringTransactionService.Setup(recurringTransaction);

            return RedirectToAction("List", "Cashflow");
        }

        [HttpGet]
        public ActionResult EditRecurringTransaction(int id)
        {
            ViewBag.Jobs = _jobRepository
                .GetAll()
                .OrderBy(j => j.Name);

            var recurringTransaction = _recurringTransactionRepository.GetById(id);

            var transaction = recurringTransaction.Transactions
                .FirstOrDefault();

            if (transaction == null)
                throw new NullReferenceException();

            var recurrintTransactionModel = new RecurringTransactionModels
            {
                Amount = transaction.Amount,
                JobId = transaction.JobId,
                Description = transaction.Description,
                NumberOfEntries = recurringTransaction.NumberOfEntries,
                Occurence = recurringTransaction.Occurence,
                StartingDate = recurringTransaction.StartingDate,
                TransactionType = transaction.TransactionType,
                RecurringTransactionId = recurringTransaction.RecurringTransactionId
            };
            
            return View(recurrintTransactionModel);
        }

        [HttpPost]
        public ActionResult EditRecurringTransaction(RecurringTransactionModels recurringTransactionModels)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Jobs = _jobRepository
                    .GetAll()
                    .OrderBy(j => j.Name);

                return View(recurringTransactionModels);
            }

            _recurringTransactionService.Edit(recurringTransactionModels);

            return RedirectToAction("List", "Cashflow");
        }

        public ActionResult DeleteRecurringTransaction(int id)
        {
            _recurringTransactionService.Delete(id);

            return RedirectToAction("RecurringTransactionList");
        }

        public ActionResult ViewRecurringTransactions(int id)
        {
            var transactions = _recurringTransactionRepository
                .GetById(id)
                .Transactions
                .ToList();

            return View(transactions);
        }
    }
}