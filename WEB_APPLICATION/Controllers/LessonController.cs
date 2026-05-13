using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class LessonController : Controller
    {
        // GET: Lessons by Course
        public ActionResult Index(int courseId)
        {
            List<Lesson> lessons = new LessonDAL().GetLessonsByCourse(courseId);
            ViewBag.CourseId = courseId;
            return View(lessons);
        }

        // GET: Lesson Details
        public ActionResult Details(int id)
        {
            Lesson lesson = new LessonDAL().GetLessonById(id);
            if (lesson == null)
                return HttpNotFound();

            List<Assessment> assessments = new AssessmentDAL().GetAssessmentsByLesson(id);
            ViewBag.Assessment = assessments != null && assessments.Count > 0 ? assessments[0] : null;

            // Progress tracking for students
            if (Session["role"] != null && Session["role"].ToString() == "student")
            {
                int userId = (int)Session["userId"];
                LessonCompletionDAL completionDAL = new LessonCompletionDAL();
                ViewBag.IsCompleted = completionDAL.IsCompleted(userId, id);
                ViewBag.CompletedCount = completionDAL.GetCompletedCount(userId, lesson.courseId);
                ViewBag.TotalLessons = new LessonDAL().GetLessonCountByCourse(lesson.courseId);
            }

            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkComplete(int lessonId, int courseId)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["role"] == null || Session["role"].ToString() != "student")
            {
                TempData["error"] = "Only students can mark lessons as complete";
                return RedirectToAction("Details", new { id = lessonId });
            }

            int userId = (int)Session["userId"];

            LessonCompletionDAL completionDAL = new LessonCompletionDAL();

            // Check if already completed
            if (!completionDAL.IsCompleted(userId, lessonId))
            {
                completionDAL.MarkComplete(userId, lessonId, courseId);

                // Recalculate course completion percentage
                EnrollmentDAL enrollmentDAL = new EnrollmentDAL();
                enrollmentDAL.RecalculateCompletionRate(userId, courseId);

                TempData["success"] = "Lesson marked as complete!";
            }
            else
            {
                TempData["success"] = "Lesson already completed";
            }

            return RedirectToAction("Details", new { id = lessonId });
        }



        // GET: Create Lesson (instructor/admin only)
        [HttpGet]
        public ActionResult Create(int courseId)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index", "Course");
            ViewBag.CourseId = courseId;
            return View();
        }

        // POST: Create Lesson
        [HttpPost]
        public ActionResult Create(int courseId, string lessonTitle, string lessonContent)
        {
            try
            {
                Lesson lesson = new Lesson(0, courseId, lessonTitle, lessonContent);
                bool success = new LessonDAL().CreateLesson(lesson);
                if (success)
                {
                    TempData["success"] = "Lesson created successfully";
                    return RedirectToAction("Index", new { courseId });
                }
                ViewBag.Error = "Failed to create lesson";
                return View();
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // GET: Edit Lesson
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Login", "Account");
            Lesson lesson = new LessonDAL().GetLessonById(id);
            if (lesson == null)
                return HttpNotFound();
            return View(lesson);
        }

        // POST: Edit Lesson
        [HttpPost]
        public ActionResult Edit(int lessonId, int courseId, string lessonTitle, string lessonContent)
        {
            try
            {
                Lesson lesson = new Lesson(lessonId, courseId, lessonTitle, lessonContent);
                bool success = new LessonDAL().UpdateLesson(lesson);
                if (success)
                {
                    TempData["success"] = "Lesson updated successfully";
                    return RedirectToAction("Details", new { id = lessonId });
                }
                ViewBag.Error = "Failed to update lesson";
                return View(lesson);
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // POST: Delete Lesson
        [HttpPost]
        public ActionResult Delete(int id, int courseId)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Login", "Account");
            bool success = new LessonDAL().DeleteLesson(id);
            if (success)
                TempData["success"] = "Lesson deleted successfully";
            return RedirectToAction("Index", new { courseId });
        }
    }
}