using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.Models;

namespace ProjectManger.WebUI.Controllers
{
    public class JobController : Controller
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<Project> _projectRepository;

        public JobController(IRepository<Job> jobRepository, IRepository<Project> projectRepository)
        {
            this._jobRepository = jobRepository;
            this._projectRepository = projectRepository;
        }

        public ActionResult Index()
        {
            var jobs = _jobRepository
                .GetAll()
                .OrderBy(j => j.Project.Name)
                .ThenBy(j => j.StartDate);

            return View(jobs);
        }

        [HttpGet]
        public ActionResult CreateJob(int id)
        {
            ViewBag.Projects = new List<Project> {_projectRepository.GetById(id)};

            return View();
        }

        [HttpPost]
        public ActionResult CreateJob(Job job)
        {
            if (ModelState.IsValid)
            {
                _jobRepository.Insert(job);
                _jobRepository.Commit();

                return RedirectToAction("JobDetails", "Job", new {id = job.JobId});
            }

            ViewBag.Projects = new List<Project> { _projectRepository.GetById(job.ProjectId) };
            return View(job);
        }

        public ActionResult JobDetails(int id)
        {
            var job = _jobRepository.GetById(id);

            return View(job);
        }

        public ActionResult DeleteJob(int id)
        {
            var job = _jobRepository.GetById(id);

            _jobRepository.Delete(id);
            _jobRepository.Commit();

            return RedirectToAction("ProjectDetails", "Project", new {id = job.ProjectId});
        }

        [HttpGet]
        public ActionResult EditJob(int id)
        {
            var job = _jobRepository.GetById(id);

            ViewBag.Projects = new List<Project> {job.Project};

            return View(job);
        }

        [HttpPost]
        public ActionResult EditJob(Job job)
        {
            if (ModelState.IsValid)
            {
                _jobRepository.Update(job);
                _jobRepository.Commit();

                return RedirectToAction("ProjectDetails", "Project", new {id = job.ProjectId});
            }

            ViewBag.Projects = new List<Project> {_projectRepository.GetById(job.ProjectId)};
            return View(job);
        }
    }
}