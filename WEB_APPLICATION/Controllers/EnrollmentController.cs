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
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["userId"];
            List<EnrollmentRecord> enrollments = new EnrollmentDAL().GetEnrollmentByUser(userId);

            CourseDAL courseDAL = new CourseDAL();
            List<Course> courses = new List<Course>();
            foreach (var enrollment in enrollments)
            {
                Course course = courseDAL.GetCourseById(enrollment.courseId);
                if (course != null)
                    courses.Add(course);
            }

            ViewBag.Courses = courses;
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