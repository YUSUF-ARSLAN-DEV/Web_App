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
            List<Lesson> lessons = LessonDAL.GetLessonsByCourse(courseId);
            ViewBag.CourseId = courseId;
            return View(lessons);
        }

        // GET: Lesson Details
        public ActionResult Details(int id)
        {
            Lesson lesson = LessonDAL.GetLessonById(id);
            if (lesson == null)
                return HttpNotFound();
            return View(lesson);
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
                Lesson lesson = new Lesson { CourseID = courseId, LessonTitle = lessonTitle, LessonContent = lessonContent };
                bool success = LessonDAL.CreateLesson(lesson);
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
                return RedirectToAction("Index");
            Lesson lesson = LessonDAL.GetLessonById(id);
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
                Lesson lesson = new Lesson { LessonID = lessonId, CourseID = courseId, LessonTitle = lessonTitle, LessonContent = lessonContent };
                bool success = LessonDAL.UpdateLesson(lesson);
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
                return Json(new { success = false });
            bool success = LessonDAL.DeleteLesson(id);
            if (success)
                TempData["success"] = "Lesson deleted successfully";
            return RedirectToAction("Index", new { courseId });
        }
    }
}