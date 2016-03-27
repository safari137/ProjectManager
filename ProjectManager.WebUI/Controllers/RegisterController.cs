using System.Linq;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models.Xero;
using ProjectManager.Services.XeroService.Payroll;

namespace ProjectManager.WebUI.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IRepository<Employee> _employeeRepository = new EmployeeRepository(new DataContext()); 
        // GET: Register
        public ActionResult Index()
        {
            return RedirectToAction("RegisterEmployees");
        }

        [HttpGet]
        public ActionResult RegisterEmployees()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterEmployees(bool success=true)
        {
            foreach (var employee in EmployeeService.GetAllEmployees())
            {
                var employeeQuery = _employeeRepository
                    .GetAll()
                    .SingleOrDefault(e => e.XeroEmployeeId == employee.XeroEmployeeId);

                if (employeeQuery != null) continue;
                var newEmployee = new Employee
                {
                    FullName = employee.FullName,
                    XeroEmployeeId = employee.XeroEmployeeId
                };

                _employeeRepository.Insert(newEmployee);
            }

            var records = _employeeRepository.Commit();

            ViewBag.Message = "Updated All Employees. " + records + " records updated.";

            return View();
        }
    }
}