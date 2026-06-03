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
        // GET: Admin Dashboard loads the admin Dashboard Page 
        public ActionResult Dashboard()
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");

            UserDAL userDal = new UserDAL();
            CourseDAL courseDal = new CourseDAL();
            EnrollmentDAL enrollmentDal = new EnrollmentDAL();
            LessonDAL lessonDal = new LessonDAL();

            var students = userDal.GetUsersByRole("student");
            var instructors = userDal.GetUsersByRole("instructor");
            var allCourses = courseDal.GetAllCourses();
            var activeCourses = courseDal.GetAllActiveCourses();

            int totalEnrollments = 0;
            int totalLessons = 0;

            if (allCourses != null)
            {
                foreach (var c in allCourses)
                {
                    var enrolls = enrollmentDal.GetEnrollmentByCourse(c.courseId);
                    if (enrolls != null)
                        totalEnrollments += enrolls.Count;
                    totalLessons += lessonDal.GetLessonCountByCourse(c.courseId);
                }
            }

            ViewBag.StudentCount = students != null ? students.Count : 0;
            ViewBag.InstructorCount = instructors != null ? instructors.Count : 0;
            ViewBag.TotalCoursesCount = allCourses != null ? allCourses.Count : 0;
            ViewBag.ActiveCoursesCount = activeCourses != null ? activeCourses.Count : 0;
            ViewBag.DeletedCoursesCount = (allCourses != null ? allCourses.Count : 0) - (activeCourses != null ? activeCourses.Count : 0);
            ViewBag.TotalEnrollments = totalEnrollments;
            ViewBag.TotalLessons = totalLessons;
            ViewBag.Students = students;
            ViewBag.Instructors = instructors;

            return View();
        }

        // GET: Manage Users (filter by role from combo box)  - the combo box in the .cshtml will pass the role and active status of users now we have 6 options 
        // instructor active / inactive   student active/inactive and all active / inactive
        public ActionResult ManageUsers(string role = "all", string status = "active")
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");

            UserDAL userDal = new UserDAL();
            List<User> users;

            // Pass both role and status to DAL
            users = userDal.GetUsersByRoleAndStatus(role, status);

            ViewBag.SelectedRole = role;
            ViewBag.SelectedStatus = status;
            return View(users);
        }

         // basically this is the revive button for the user , since before we could only delete a user from existen but now they could be revieved 
         // aka their activestatus changing from 0 to 1 again . 
        [HttpPost]
        public ActionResult ActivateUser(int userId)
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");

            bool success = new UserDAL().Activate(userId);
            if (success)
                TempData["success"] = "User activated successfully";
            else
                TempData["Error"] = "Failed to activate user";

            return RedirectToAction("ManageUsers", new { role = Request.QueryString["role"], status = Request.QueryString["status"] });
        }

        // POST: Deleting a user 
        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");

            int currentAdminId = (int)Session["userId"];
            if (userId == currentAdminId)
            {
                TempData["Error"] = "You cannot delete your own account";
                return RedirectToAction("ManageUsers");
            }

            bool success = new UserDAL().DeleteUser(userId);
            if (success)
                TempData["success"] = "User deleted successfully";
            else
                TempData["Error"] = "Failed to delete user";

            return RedirectToAction("ManageUsers");
        }

        // GET: Managing courses basically allowing the admin to view all existing courses in the platform , and deleteing which courses that he thinks is unappropriate 
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
                return RedirectToAction("Login", "Account");
            new CourseDAL().DeleteCourse(courseId);
            return RedirectToAction("ManageCourses");
        }
        // so that an admin is able to reactive a course that was delete before instead of deleting it permanently from the database , this is done by changing the active status of the course from 0 to 1 again 
        [HttpPost]
        public ActionResult ReactivateCourse(int courseId)
        {
            if (Session["userId"] == null || !IsAdmin())
                return RedirectToAction("Login", "Account");

            new CourseDAL().ReactivateCourse(courseId);
            TempData["success"] = "Course reactivated successfully";
            return RedirectToAction("ManageCourses");
        }

        private bool IsAdmin() // checks if the logged in user is an admin and returns a boolean value accordingly 
        {
            return Session["role"] != null && Session["role"].ToString() == "admin";
        }
    }
}