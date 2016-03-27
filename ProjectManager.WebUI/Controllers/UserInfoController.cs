using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models;
using ProjectManager.Services;

namespace ProjectManager.WebUI.Controllers
{
    [Authorize(Roles = "viewer, manager, admin")]
    public class UserInfoController : Controller
    {
        private readonly UserService _userService = new UserService(new AppUserRepository(new DataContext()));

        public ActionResult Index()
        {
            var userInfo = _userService.GetUserInfo(User.Identity.GetUserName());

            return View(userInfo);
        }

        [HttpPost]
        public ActionResult UpdateUser(AppUser user)
        {
            _userService.UpdateUser(user);

            return RedirectToAction("Index");
        }
    }
}