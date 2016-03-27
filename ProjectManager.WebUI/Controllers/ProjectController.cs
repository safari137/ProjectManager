using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models;

namespace ProjectManager.WebUI.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Customer> _customerRepository; 

        public ProjectController(IRepository<Project> projectRepository, IRepository<Customer> customerRepository)
        {
            this._projectRepository = projectRepository;
            this._customerRepository = customerRepository;
        }
        
        public ActionResult Index()
        {
            var projects = _projectRepository
                .GetAll()
                .OrderBy(p => p.StartDate)
                .ThenBy(p => p.FinishDate);

            return View(projects);
        }

        [HttpGet]
        public ActionResult CreateProject(int id)
        {
            ViewBag.Customers = new List<Customer>()
            {
                _customerRepository.GetById(id)
            };

            return View();
        }

        [HttpPost]
        public ActionResult CreateProject(Project project)
        {
            if (ModelState.IsValid)
            {
                _projectRepository.Insert(project);
                _projectRepository.Commit();

                return RedirectToAction("ProjectDetails", "Project", new {id = project.ProjectId});
            }

            ViewBag.Customers = new List<Customer> {_customerRepository.GetById(project.CustomerId)};

            return View(project);
        }

        public ActionResult ProjectDetails(int id)
        {
            var project = _projectRepository.GetById(id);
                
            return View(project);
        }

        public ActionResult DeleteProject(int id)
        {
            var customerId = _projectRepository.GetById(id).CustomerId;

            _projectRepository.Delete(id);
            _projectRepository.Commit();

            return RedirectToAction("CustomerDetails", "Customer", new { id = customerId });
        }

        public ActionResult EditProject(int id)
        {
            var project = _projectRepository.GetById(id);
            
            ViewBag.Customers = _customerRepository
                .GetAll()
                .OrderBy(c => c.Name);

            return View(project);
        }

        [HttpPost]
        public ActionResult EditProject(Project project)
        {
            if (ModelState.IsValid)
            {
                _projectRepository.Update(project);
                _projectRepository.Commit();

                return RedirectToAction("ProjectDetails", new {id = project.ProjectId});
            }

            ViewBag.Customers = _customerRepository
                .GetAll()
                .OrderBy(c => c.Name);

            return View(project);

        }
    }
}