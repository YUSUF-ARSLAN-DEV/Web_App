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
            List<Course> courses = CourseDAL.GetAllCourses();
            return View(courses);
        }

        // GET: Course Details
        public ActionResult Details(int id)
        {
            Course course = CourseDAL.GetCourseById(id);
            if (course == null)
                return HttpNotFound();
            return View(course);
        }

        // GET: My Courses (for enrolled students)
        public ActionResult MyCourses()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            int userId = (int)Session["userId"];
            // TODO: implement enrollment query when EnrollmentDAL method exists
            return View();
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
        public ActionResult Create(string courseTitle, string courseDescription)
        {
            try
            {
                Course course = new Course { CourseTitle = courseTitle, CourseDescription = courseDescription };
                bool success = CourseDAL.CreateCourse(course);
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
            Course course = CourseDAL.GetCourseById(id);
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
                Course course = new Course { CourseID = id, CourseTitle = courseTitle, CourseDescription = courseDescription };
                bool success = CourseDAL.UpdateCourse(course);
                if (success)
                {
                    TempData["success"] = "Course updated successfully";
                    return RedirectToAction("Details", new { id });
                }
                ViewBag.Error = "Failed to update course";
                return View(course);
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
            bool success = CourseDAL.DeleteCourse(id);
            if (success)
                TempData["success"] = "Course deleted successfully";
            return RedirectToAction("Index");
        }
    }
}