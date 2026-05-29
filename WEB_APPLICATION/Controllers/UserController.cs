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

            string userName = (string)Session["userName"];
            UserDAL userDal = new UserDAL();
            int userId = (int)Session["userId"];
            User user = userDal.GetUserById(userId);

            // Check if user has no password (OAuth signup)
            if (string.IsNullOrEmpty(user.password))
            {
                // Skip current password check, just validate new password
                if (!userDal.CheckValidCredentials(userName, newPassword))
                {
                    ViewBag.Error = "New password does not meet requirements. Must contain uppercase, lowercase, and a number.";
                    return View();
                }

                bool success = userDal.UpdatePassword(userId, newPassword);
                if (success)
                {
                    TempData["success"] = "Password created successfully. You can now log in using your username and password.";
                    return RedirectToAction("Profile");
                }

                ViewBag.Error = "Failed to create password";
                return View();
            }

            // Normal flow for users with existing password
            int authResult = userDal.LoginAuthentication(userName, currentPassword);
            if (authResult != 0)
            {
                ViewBag.Error = "Current password is incorrect";
                return View();
            }

            if (!userDal.CheckValidCredentials(userName, newPassword))
            {
                ViewBag.Error = "New password does not meet requirements. Must contain uppercase, lowercase, and a number.";
                return View();
            }

            bool successNormal = userDal.UpdatePassword(userId, newPassword);
            if (successNormal)
            {
                TempData["success"] = "Password changed successfully";
                return RedirectToAction("Profile");
            }

            ViewBag.Error = "Failed to change password";
            return View();
        }
    }
}