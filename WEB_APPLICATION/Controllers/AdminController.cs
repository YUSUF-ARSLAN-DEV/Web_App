using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin Dashboard
        public ActionResult Dashboard()
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");
            return View();
        }

        // GET: Manage Users (filter by role from combo box)
        public ActionResult ManageUsers(string role = "all")
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");

            UserDAL userDal = new UserDAL();
            List<User> users;

            if (role == "student")
            {
                users = userDal.GetUsersByRole("student");
            }
            else if (role == "instructor")
            {
                users = userDal.GetUsersByRole("instructor");
            }
            else
            {
                users = userDal.GetAllActiveNonAdminUsers();
            }

            ViewBag.SelectedRole = role;
            return View(users);
        }

        // POST: Delete User
        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            if (Session["userId"] == null || !IsAdmin())
                return Json(new { success = false });

            // Prevent admin from deleting themselves
            int currentAdminId = (int)Session["userId"];
            if (userId == currentAdminId)
                return Json(new { success = false, message = "You cannot delete your own account" });

            bool success = new UserDAL().DeleteUser(userId);
            return Json(new { success = success });
        }

        // GET: Manage Courses
        public ActionResult ManageCourses()
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");
            List<Course> courses = new CourseDAL().GetAllCourses();
            return View(courses);
        }

        // POST: Delete Course
        [HttpPost]
        public ActionResult DeleteCourse(int courseId)
        {
            if (Session["userId"] == null || !IsAdmin())
                return Json(new { success = false });
            bool deleted = new CourseDAL().DeleteCourse(courseId);
            return Json(new { success = deleted });
        }

        private bool IsAdmin()
        {
            return Session["role"] != null && Session["role"].ToString() == "admin";
        }
    }
}