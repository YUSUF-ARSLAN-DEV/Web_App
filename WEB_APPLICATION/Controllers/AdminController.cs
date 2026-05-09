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

        // GET: Manage Users
        public ActionResult ManageUsers()
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");
            List<User> users = UserDAL.getAllUsers("student");
            return View(users);
        }

        // POST: Delete User
        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            if (Session["userId"] == null || !IsAdmin())
                return Json(new { success = false });
            // TODO: implement user deletion logic when UserDAL method exists
            return Json(new { success = true });
        }

        // GET: Manage Courses
        public ActionResult ManageCourses()
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");
            List<Course> courses = CourseDAL.GetAllCourses();
            return View(courses);
        }

        // POST: Delete Course
        [HttpPost]
        public ActionResult DeleteCourse(int courseId)
        {
            if (Session["userId"] == null || !IsAdmin())
                return Json(new { success = false });
            bool deleted = CourseDAL.DeleteCourse(courseId);
            return Json(new { success = deleted });
        }

        private bool IsAdmin()
        {
            return Session["role"] != null && Session["role"].ToString() == "admin";
        }
    }
}