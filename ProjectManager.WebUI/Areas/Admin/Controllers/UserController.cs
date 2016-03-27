using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models;
using ProjectManager.Services;
using ProjectManager.WebUI.Infrastructure;

namespace ProjectManager.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("Admin")]
    public class UserController : Controller
    {
        private readonly IRepository<AppUser> _userRepository;
        private readonly UserService _userService;

        public UserController()
        {
            _userRepository = new AppUserRepository(new DataContext());
            _userService = new UserService(_userRepository);
        }

        public ActionResult Index()
        {
            var users = _userRepository.GetAll();

            return View(users);
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(AppUser user)
        {
            if (!ModelState.IsValid)
                return View(user);

            if (_userService.CreateUser(user.Username, user.Password, user.Email, user.Role))
                return RedirectToAction("Index");

            return View(user);
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var user = _userRepository.GetById(id);

            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(AppUser user)
        {
            if (!ModelState.IsValid)
                return View(user);

            _userRepository.Update(user);
            _userRepository.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult DeleteUser(int id)
        {
            _userRepository.Delete(id);
            _userRepository.Commit();

            return RedirectToAction("Index");
        }
    }
}