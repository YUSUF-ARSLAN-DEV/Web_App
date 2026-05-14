using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class EnrollmentController : Controller
    {
        // gets the list of Enrollments to courses and shows Course Names 
        public ActionResult MyEnrollments()
        {
            int userId = (int)Session["userId"];

            // Force recalculate for all courses
            var enrollments = new EnrollmentDAL().GetEnrollmentByUser(userId);
            foreach (var e in enrollments)
            {
                new EnrollmentDAL().RecalculateCompletionRate(userId, e.courseId);
            }

            // Then get fresh data
            enrollments = new EnrollmentDAL().GetEnrollmentByUser(userId);
            ViewBag.Courses = new CourseDAL().GetAllActiveCourses();

            return View(enrollments);
        }

        // POST: Enroll in Course
        [HttpPost]
        public ActionResult Enroll(int courseId)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["userId"];
            EnrollmentDAL dal = new EnrollmentDAL();

            if (dal.IsEnrolled(userId, courseId))
            {
                TempData["Error"] = "You are already enrolled in this course";
                return RedirectToAction("Details", "Course", new { id = courseId });
            }

            bool success = dal.Enroll(userId, courseId);
            if (success)
                TempData["success"] = "Successfully enrolled!";
            else
                TempData["Error"] = "Enrollment failed. Please try again.";

            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        // POST: Unenroll from Course
        [HttpPost]
        public ActionResult Unenroll(int enrollmentId, int courseId)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["userId"];
            new EnrollmentDAL().UnEnroll(userId, courseId);
            return RedirectToAction("MyEnrollments");
        }
    }
}