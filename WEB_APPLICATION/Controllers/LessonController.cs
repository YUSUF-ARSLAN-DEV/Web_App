
using System;
using System.Collections.Generic;
using System.IO;
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
                ViewBag.TestMessage = "Student block entered";

                int userId = (int)Session["userId"];
                LessonCompletionDAL completionDAL = new LessonCompletionDAL();
                EnrollmentDAL enrollmentDAL = new EnrollmentDAL();

                bool isEnrolled = enrollmentDAL.IsEnrolled(userId, lesson.courseId);
                ViewBag.IsEnrolled = isEnrolled;

                ViewBag.TestMessage = $"Student block: IsEnrolled = {isEnrolled}";

                ViewBag.IsCompleted = completionDAL.IsCompleted(userId, id);
                ViewBag.CompletedCount = completionDAL.GetCompletedCount(userId, lesson.courseId);
                ViewBag.TotalLessons = new LessonDAL().GetLessonCountByCourse(lesson.courseId);
            }
            else
            {
                ViewBag.TestMessage = "Student block NOT entered - Role = " + Session["role"];
                ViewBag.IsEnrolled = false;
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
            Course course = new CourseDAL().GetCourseById(courseId);
            if (course != null && !course.activeStatus)
            {
                TempData["Error"] = "Cannot add lessons to an archived course.";
                return RedirectToAction("Details", "Course", new { id = courseId });
            }
            ViewBag.CourseId = courseId;
            return View();
        }

        // POST: Create Lesson Controller
        [HttpPost]
        public ActionResult Create(int courseId, string lessonTitle, string lessonContent, string videoUrl = null, HttpPostedFileBase attachment = null)
        {
            try
            {
                Lesson lesson = new Lesson(0, courseId, lessonTitle, lessonContent, videoUrl);
                if (string.IsNullOrWhiteSpace(lessonTitle))
                {
                    ViewBag.Error = "Lesson title is required.";
                    return View();
                }
                if (string.IsNullOrWhiteSpace(lessonContent))
                {
                    ViewBag.Error = "Lesson content is required.";
                    return View();
                }
                if (attachment != null && attachment.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(attachment.FileName);
                    string uniqueName = Guid.NewGuid().ToString() + "_" + fileName;
                    lesson.attachmentUrl = BlobStorageHelper.UploadFile(attachment.InputStream, uniqueName, "documents");
                    lesson.attachmentName = fileName;
                }
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
        public ActionResult Edit(int lessonId, int courseId, string lessonTitle, string lessonContent, string videoUrl = null, HttpPostedFileBase attachment = null, bool removeAttachment = false)
        {
            try
            {
                LessonDAL lessonDal = new LessonDAL();
                Lesson existingLesson = lessonDal.GetLessonById(lessonId);
                if (string.IsNullOrWhiteSpace(lessonTitle))
                {
                    ViewBag.Error = "Lesson title is required.";
                    return View();
                }
                if (string.IsNullOrWhiteSpace(lessonContent))
                {
                    ViewBag.Error = "Lesson content is required.";
                    return View();
                }
                if (existingLesson == null)
                {
                    TempData["error"] = "Lesson not found";
                    return RedirectToAction("Index", new { courseId });
                }
                Lesson lesson = new Lesson(lessonId, courseId, lessonTitle, lessonContent, videoUrl);
                if (removeAttachment)
                {
                    if (!string.IsNullOrEmpty(existingLesson.attachmentUrl))
                    {
                        string oldFileName = Path.GetFileName(existingLesson.attachmentUrl);
                        BlobStorageHelper.DeleteFile(oldFileName, "documents");
                    }
                    lesson.attachmentUrl = null;
                    lesson.attachmentName = null;
                }
                else if (attachment != null && attachment.ContentLength > 0)
                {
                    // Delete old attachment if exists
                    if (!string.IsNullOrEmpty(existingLesson.attachmentUrl))
                    {
                        string oldFileName = Path.GetFileName(existingLesson.attachmentUrl);
                        BlobStorageHelper.DeleteFile(oldFileName, "documents");
                    }
                    string fileName = Path.GetFileName(attachment.FileName);
                    string uniqueName = Guid.NewGuid().ToString() + "_" + fileName;
                    lesson.attachmentUrl = BlobStorageHelper.UploadFile(attachment.InputStream, uniqueName, "documents");
                    lesson.attachmentName = fileName;
                }
                else
                {
                    lesson.attachmentUrl = existingLesson.attachmentUrl;
                    lesson.attachmentName = existingLesson.attachmentName;
                }
                bool success = lessonDal.UpdateLesson(lesson);
                if (success)
                {
                    TempData["success"] = "Lesson updated successfully";
                    return RedirectToAction("Details", new { id = lessonId });
                }
                ViewBag.Error = "Failed to update lesson";
                return View(lesson);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                ViewBag.Error = "An error occurred";
                return View();
            }
        }
        public ActionResult DownloadAttachment(int id) // a controller metod to download the uploaded attachment 
        {
            // Check if user is logged in
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            LessonDAL lessonDal = new LessonDAL();
            Lesson lesson = lessonDal.GetLessonById(id);

            if (lesson == null)
            {
                TempData["error"] = "Lesson not found";
                return RedirectToAction("Index", "Home");
            }

            // Check if attachment exists
            if (string.IsNullOrEmpty(lesson.attachmentUrl))
            {
                TempData["error"] = "No attachment found for this lesson";
                return RedirectToAction("Details", new { id = id });
            }

            string filePath = Server.MapPath(lesson.attachmentUrl);

            // Check if file exists on disk
            if (!System.IO.File.Exists(filePath))
            {
                TempData["error"] = "Attachment file not found on server";
                return RedirectToAction("Details", new { id = id });
            }

            // For students, check if they are enrolled
            if (Session["role"] != null && Session["role"].ToString() == "student")
            {
                EnrollmentDAL enrollmentDal = new EnrollmentDAL();
                int userId = (int)Session["userId"];

                if (!enrollmentDal.IsEnrolled(userId, lesson.courseId))
                {
                    TempData["error"] = "You must be enrolled to download attachments";
                    return RedirectToAction("Details", "Course", new { id = lesson.courseId });
                }
            }

            // Return file for download
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", lesson.attachmentName);
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