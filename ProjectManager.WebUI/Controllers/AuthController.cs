using System;
using System.Web.Mvc;
using System.Web.Security;
using ProjectManager.Models;
using ProjectManager.Services;
using ProjectManager.DAL.Repositories;
using ProjectManager.DAL.Data;

namespace ProjectManager.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _userService = new UserService(new AppUserRepository(new DataContext()));
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AppUser appUser, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(appUser);

            if (!_userService.UserIsValid(appUser.Username, appUser.Password))
                return View(appUser);

            FormsAuthentication.SetAuthCookie(appUser.Username, true);

            _userService.SetLastLogin(appUser.Username, DateTime.Now.AddHours(3));

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}