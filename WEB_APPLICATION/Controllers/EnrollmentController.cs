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
                return Json(new { success = false, message = "Not logged in" });
            
            int userId = (int)Session["userId"];
            EnrollmentDAL dal = new EnrollmentDAL();
            if (dal.IsEnrolled(userId, courseId))
            {
                return Json(new { success = false, message = "Already enrolled" });
            }
            bool success = dal.Enroll(userId, courseId);
            return Json(new { success = success });
        }

        // POST: Unenroll from Course
        [HttpPost]
        public ActionResult Unenroll(int enrollmentId, int courseId)
        {
            if (Session["userId"] == null)
                return Json(new { success = false, message = "Not logged in" });
            
            int userId = (int)Session["userId"];
            bool success = new EnrollmentDAL().UnEnroll(userId, courseId);
            return Json(new { success = success });
        }
    }
}