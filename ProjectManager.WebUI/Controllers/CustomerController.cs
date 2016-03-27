using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.Models;

namespace ProjectManager.WebUI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerController(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public ActionResult Index()
        {
            var customers = _customerRepository
                .GetAll()
                .OrderBy(c => c.Name);

            return View(customers);
        }

        public ActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _customerRepository.Insert(customer);
                _customerRepository.Commit();

                return RedirectToAction("CustomerDetails", new {id = customer.CustomerId});
            }

            return View(customer);
        }

        public ActionResult CustomerDetails(int id)
        {
            var customer = _customerRepository.GetById(id);

            return View(customer);
        }

        public ActionResult DeleteCustomer(int id)
        {
            _customerRepository.Delete(id);
            _customerRepository.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult EditCustomer(int id)
        {
            var customer = _customerRepository.GetById(id);

            return View(customer);
        }

        [HttpPost]
        public ActionResult EditCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            _customerRepository.Update(customer);
            _customerRepository.Commit();

            return RedirectToAction("Index");
        }
    }
}