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
        // GET: My Enrollments
        public ActionResult MyEnrollments()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            int userId = (int)Session["userId"];
            // TODO: implement enrollment list query
            return View();
        }

        // POST: Enroll in Course
        [HttpPost]
        public ActionResult Enroll(int courseId)
        {
            if (Session["userId"] == null)
                return Json(new { success = false, message = "Not logged in" });
            
            int userId = (int)Session["userId"];
            // TODO: implement enrollment creation when EnrollmentDAL exists
            return Json(new { success = true, message = "Enrolled successfully" });
        }

        // POST: Unenroll from Course
        [HttpPost]
        public ActionResult Unenroll(int enrollmentId)
        {
            if (Session["userId"] == null)
                return Json(new { success = false, message = "Not logged in" });
            
            // TODO: implement enrollment deletion when EnrollmentDAL exists
            return Json(new { success = true, message = "Unenrolled successfully" });
        }
    }
}