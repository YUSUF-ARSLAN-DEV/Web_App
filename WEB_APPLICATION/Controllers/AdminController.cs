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
                return RedirectToAction("Login", "Account");
            new CourseDAL().DeleteCourse(courseId);
            return RedirectToAction("ManageCourses");
        }

        private bool IsAdmin()
        {
            return Session["role"] != null && Session["role"].ToString() == "admin";
        }
    }
}