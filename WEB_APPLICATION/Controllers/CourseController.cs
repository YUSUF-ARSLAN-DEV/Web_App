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
        public ActionResult Index(string search )
        { //every time the Course Home page  is loaded if the search bar has a word it will filter the courses based on that word 
            List<Course> courses;
            if (!string.IsNullOrEmpty(search))
                courses = new CourseDAL().FilterCourses(search);
            else
                courses = new CourseDAL().GetAllActiveCourses();
            return View(courses);
        }

        // GET: Course Details // showing the details of a course nad its rating 
        public ActionResult Details(int id)
        {
            Course course = new CourseDAL().GetCourseById(id);
            if (course == null)
                return HttpNotFound();

            RatingDAL ratingDal = new RatingDAL();
            ViewBag.AverageRating = ratingDal.GetAverageRating(id);
            ViewBag.Ratings = ratingDal.GetRatingsByCourse(id);
            ViewBag.Lessons = new LessonDAL().GetLessonsByCourse(id);

            if (Session["userId"] != null && Session["role"].ToString() == "student")
            {
                int userId = (int)Session["userId"];
                ViewBag.HasRated = new RatingDAL().HasUserRated(userId, id);
                  ViewBag.IsEnrolled = new EnrollmentDAL().IsEnrolled(userId, id); // to check if the student is enrolled or not so that he gets shown the corerct button 
                var enrollment = new EnrollmentDAL().GetEnrollment(userId, id);
                ViewBag.EnrollmentId = enrollment != null ? enrollment.enrollmentId : 0;
            }
            else
            {
                ViewBag.IsEnrolled = false;
            }

            return View(course);
        }
        [HttpGet]
        public ActionResult Rate(int id) //gets the rating of a course 
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            
            Course course = new CourseDAL().GetCourseById(id);
            if (course == null)
                return HttpNotFound();
            
            int userId = (int)Session["userId"];
            if (new RatingDAL().HasUserRated(userId, id))
            {
                TempData["Error"] = "You have already rated this course";
                return RedirectToAction("Details", new { id });
            }
            
            ViewBag.CourseId = id;
            return View();
        }

        // POST: Rate Course
        [HttpPost] // the students gives a rating for a course 
        public ActionResult Rate(int courseId, int score, string comment)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            
            int userId = (int)Session["userId"];
            bool success = new RatingDAL().AddRating(courseId, userId, score, comment);
            
            if (success)
            {
                TempData["success"] = "Rating submitted successfully";
                return RedirectToAction("Details", new { id = courseId });
            }
            
            ViewBag.Error = "Failed to submit rating";
            return View();
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
        public ActionResult Create(string courseName, string courseDescription, HttpPostedFileBase imageFile)
        {
            try
            {
                int userId = (int)Session["userId"];
                string imageUrl = null;
                if (string.IsNullOrWhiteSpace(courseName)) // server side validation for name and description 
                {
                    ViewBag.Error = "Course title is required.";
                    return View();
                }
                if (string.IsNullOrWhiteSpace(courseDescription))
                {
                    ViewBag.Error = "Course description is required.";
                    return View();
                }

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    string uniqueName = System.Guid.NewGuid().ToString() + "_" + fileName;
                    string savePath = Server.MapPath("~/Uploads/" + uniqueName);
                    imageFile.SaveAs(savePath);
                    imageUrl = "/Uploads/" + uniqueName;
                }

                bool success = new CourseDAL().CreateCourse(userId, courseName, courseDescription, imageUrl);
                if (success)
                {
                    TempData["success"] = "Course created successfully";
                    return RedirectToAction("Dashboard", "Instructor");
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
            if (!course.activeStatus)
            {
                TempData["Error"] = "Archived courses cannot be edited. Reactivate the course first.";
                return RedirectToAction("Details", new { id });
            }
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

        // POST: Delete Course (soft-delete → archive)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index");
            bool success = new CourseDAL().DeleteCourse(id);
            if (success)
                TempData["success"] = "Course moved to archive.";
            return RedirectToAction("Dashboard", "Instructor");
        }

        // POST: Reactivate Course
        [HttpPost]
        public ActionResult Reactivate(int id)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index");
            new CourseDAL().ReactivateCourse(id);
            TempData["success"] = "Course reactivated successfully.";
            return RedirectToAction("ArchivedCourses");
        }

        // GET: Archived Courses
        [HttpGet]
        public ActionResult ArchivedCourses()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index");
            int userId = (int)Session["userId"];
            List<Course> archived = new CourseDAL().GetDeletedCoursesByUserId(userId);
            return View(archived);
        }
    }
}