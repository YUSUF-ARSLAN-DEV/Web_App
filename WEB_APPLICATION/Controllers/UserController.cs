using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class UserController : Controller
    {
        // GET: User Profile
        public ActionResult Profile()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            int userId = (int)Session["userId"];
            User user = new UserDAL().GetUserById(userId);
            if (user == null)
                return HttpNotFound();
            return View(user);
        }

        // GET: Edit Profile
        [HttpGet]
        public ActionResult EditProfile()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            int userId = (int)Session["userId"];
            User user = new UserDAL().GetUserById(userId);
            if (user == null)
                return HttpNotFound();
            return View(user);
        }

        // POST: Edit Profile
        [HttpPost]
        public ActionResult EditProfile(string firstName, string lastName)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            int userId = (int)Session["userId"];
            UserDAL userDal = new UserDAL();
            bool success = userDal.UpdateUserProfile(userId, firstName, lastName);
            if (success)
            {
                TempData["success"] = "Profile updated successfully";
                return RedirectToAction("Profile");
            }
            ViewBag.Error = "Failed to update profile";
            return View();
        }

        // GET: Change Password
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: Change Password
        [HttpPost]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New passwords do not match";
                return View();
            }

            int userId = (int)Session["userId"];
            UserDAL userDal = new UserDAL();
            bool success = userDal.UpdatePassword(userId, newPassword);
            if (success)
            {
                TempData["success"] = "Password changed successfully";
                return RedirectToAction("Profile");
            }
            ViewBag.Error = "Failed to change password";
            return View();
        }
    }
}