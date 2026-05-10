using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course Index
        public ActionResult Index()
        {
            List<Course> courses = new CourseDAL().GetAllActiveCourses();
            return View(courses);
        }

        // GET: Course Details
        public ActionResult Details(int id)
        {
            Course course = new CourseDAL().GetCourseById(id);
            if (course == null)
                return HttpNotFound();
            return View(course);
        }

        // GET: My Courses (for enrolled students)
        [HttpGet] 
        public ActionResult MyCourses()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            
            int userId = (int)Session["userId"];
            List<EnrollmentRecord> enrollments = new EnrollmentDAL().GetEnrollmentByUser(userId);
            
            CourseDAL courseDAL = new CourseDAL();
            List<Course> courses = new List<Course>();
            
            foreach (EnrollmentRecord enrollment in enrollments)
            {
                Course course = courseDAL.GetCourseById(enrollment.courseId);
                if (course != null)
                    courses.Add(course);
            }
            
            ViewBag.Enrollments = enrollments;
            return View(courses);
        }

        // GET: Create Course (admin/instructor only)
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index");
            return View();
        }

        // POST: Create Course
        [HttpPost]
        public ActionResult Create(string courseName, string courseDescription)
        {
            try
            {
                int userId = (int)Session["userId"];
                bool success = new CourseDAL().CreateCourse(userId, courseName, courseDescription, null);
                if (success)
                {
                    TempData["success"] = "Course created successfully";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Failed to create course";
                return View();
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // GET: Edit Course
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index");
            Course course = new CourseDAL().GetCourseById(id);
            if (course == null)
                return HttpNotFound();
            return View(course);
        }

        // POST: Edit Course
        [HttpPost]
        public ActionResult Edit(int id, string courseTitle, string courseDescription)
        {
            try
            {
                bool success = new CourseDAL().UpdateCourse(id, courseTitle, courseDescription);
                if (success)
                {
                    TempData["success"] = "Course updated successfully";
                    return RedirectToAction("Details", new { id });
                }
                ViewBag.Error = "Failed to update course";
                return View();
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // POST: Delete Course
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return Json(new { success = false });
            bool success = new CourseDAL().DeleteCourse(id);
            if (success)
                TempData["success"] = "Course deleted successfully";
            return RedirectToAction("Index");
        }
    }
}