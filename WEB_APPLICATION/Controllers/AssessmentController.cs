using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class AssessmentController : Controller
    {
        // GET: Take Quiz
        public ActionResult TakeQuiz(int assessmentId)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            string role = Session["role"] != null ? Session["role"].ToString() : "";
            if (role == "instructor" || role == "admin")
            {
                Assessment a = new AssessmentDAL().GetAssessmentById(assessmentId);
                TempData["error"] = "Instructors cannot take quizzes.";
                return RedirectToAction("Details", "Lesson", new { id = a != null ? a.lessonId : 0 });
            }

            Assessment assessment = new AssessmentDAL().GetAssessmentById(assessmentId);
            if (assessment == null)
                return HttpNotFound();
            List<Question> questions = new QuestionDAL().GetQuestionsByAssessment(assessmentId);
            ViewBag.Questions = questions;
            return View(assessment);
        }

        // GET: Manage Quiz (Instructor only)
        [HttpGet]
        public ActionResult Manage(int lessonId)
        {
            if (Session["userId"] == null || Session["role"].ToString() != "instructor")
                return RedirectToAction("Login", "Account");

            Lesson lesson = new LessonDAL().GetLessonById(lessonId);
            if (lesson == null)
                return HttpNotFound();

            // Check if instructor owns this course
            Course course = new CourseDAL().GetCourseById(lesson.courseId);
            if (course.userId != (int)Session["userId"] && Session["role"].ToString() != "admin")
                return new HttpUnauthorizedResult();

            ViewBag.Lesson = lesson;

            var assessments = new AssessmentDAL().GetAssessmentsByLesson(lessonId);
            ViewBag.Assessment = assessments != null && assessments.Count > 0 ? assessments[0] : null;

            if (ViewBag.Assessment != null)
            {
                ViewBag.Questions = new QuestionDAL().GetQuestionsByAssessment(ViewBag.Assessment.assessmentId);
            }

            return View();
        }

        // POST: Delete Question
        [HttpPost]
        public ActionResult DeleteQuestion(int questionId, int assessmentId)
        {
            if (Session["role"] == null || Session["role"].ToString() != "instructor")
                return RedirectToAction("Login", "Account");

            new QuestionDAL().DeleteQuestion(questionId);
            TempData["success"] = "Question deleted successfully";
            return RedirectToAction("Manage", new { lessonId = new AssessmentDAL().GetAssessmentById(assessmentId).lessonId });
        }

        // POST: Delete Assessment (Entire Quiz)
        [HttpPost]
        public ActionResult DeleteAssessment(int id, int lessonId)
        {
            if (Session["role"] == null || Session["role"].ToString() != "instructor")
                return RedirectToAction("Login", "Account");

            new AssessmentDAL().DeleteAssessment(id);
            TempData["success"] = "Quiz deleted successfully";
            return RedirectToAction("Manage", new { lessonId = lessonId });
        }

        // POST: Submit Quiz
        [HttpPost]
        public ActionResult SubmitQuiz(int assessmentId, FormCollection answers)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            List<Question> questions = new QuestionDAL().GetQuestionsByAssessment(assessmentId);
            if (questions == null || questions.Count == 0)
                return RedirectToAction("Results", new { assessmentId });

            int correct = 0;
            int total = questions.Count;

            foreach (Question q in questions)
            {
                string submitted = answers["q_" + q.questionId];
                if (submitted == null) continue;

                if (q.questionType == "multiple_choice" || q.questionType == "true_false")
                {
                    if (submitted.Trim().Equals(q.correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                        correct++;
                }
                else if (q.questionType == "short_answer")
                {
                    System.Func<string, string> normalize = s =>
                        System.Text.RegularExpressions.Regex.Replace(s.ToLowerInvariant(), @"[^\w\s]", "").Trim();

                    string normSubmitted = normalize(submitted);
                    string normCorrect = normalize(q.correctAnswer);

                    if (normSubmitted == normCorrect)
                    {
                        correct++;
                    }
                    else if (!string.IsNullOrEmpty(q.questionAnswer))
                    {
                        string[] keywords = q.questionAnswer.Split('|');
                        foreach (string kw in keywords)
                        {
                            if (normSubmitted.Contains(normalize(kw)))
                            {
                                correct++;
                                break;
                            }
                        }
                    }
                }
            }

            new AssessmentDAL().IncrementAttempt(assessmentId);

            TempData["score"] = correct;
            TempData["total"] = total;

            // After calculating score and total
            double percentage = (double)correct / total * 100;
            int passingScore = 70; // 70% to pass

            if (percentage >= passingScore)
            {
                // Get the assessment to find lessonId
                Assessment assessment = new AssessmentDAL().GetAssessmentById(assessmentId);

                // Get the lesson to find courseId
                Lesson lesson = new LessonDAL().GetLessonById(assessment.lessonId);

                // Get current user
                int userId = (int)Session["userId"];

                // Mark lesson as complete
                LessonCompletionDAL completionDAL = new LessonCompletionDAL();
                if (!completionDAL.IsCompleted(userId, assessment.lessonId))
                {
                    completionDAL.MarkComplete(userId, assessment.lessonId, lesson.courseId);

                    // Recalculate course progress
                    new EnrollmentDAL().RecalculateCompletionRate(userId, lesson.courseId);
                }
            }
            return RedirectToAction("Results", new { assessmentId });
        }

        // GET: Quiz Results
        public ActionResult Results(int assessmentId)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            Assessment assessment = new AssessmentDAL().GetAssessmentById(assessmentId);
            if (assessment == null)
                return HttpNotFound();

            ViewBag.Score = TempData["score"];
            ViewBag.Total = TempData["total"];

            return View(assessment);
        }

        // GET: Retrives the View ( form ) inside which an Assessment will be created for a specific lesson 
        [HttpGet]
        public ActionResult Create(int lessonId)
        {
            if (Session["role"] == null || Session["role"].ToString() != "instructor")
                return RedirectToAction("Login", "Account");
            ViewBag.LessonId = lessonId;
            return View();
        }

        // POST: Create Assessment for a lesson
        [HttpPost]
        [ActionName("Create")]
        public ActionResult CreatePost(int lessonId)
        {
            if (Session["role"] == null ||  Session["role"].ToString() != "instructor")
                return RedirectToAction("Index", "Course");

            Assessment assessment = new Assessment( lessonId);
            bool success = new AssessmentDAL().CreateAssessment(assessment);
            if (success)
            {
                List<Assessment> assessments = new AssessmentDAL().GetAssessmentsByLesson(lessonId);
                Assessment created = assessments[assessments.Count - 1];
                TempData["success"] = "Quiz created! Now add your questions.";
                return RedirectToAction("AddQuestion", new { assessmentId = created.assessmentId });
            }
            ViewBag.Error = "Failed to create assessment";
            ViewBag.LessonId = lessonId;
            return View();
        }

        // GET: Add Question
        [HttpGet]
        public ActionResult AddQuestion(int assessmentId)
        {
            if (Session["role"] == null || Session["role"].ToString() != "instructor")
                return RedirectToAction("Login", "Account");
            ViewBag.AssessmentId = assessmentId;
            Assessment assessment = new AssessmentDAL().GetAssessmentById(assessmentId);
            ViewBag.LessonId = assessment != null ? assessment.lessonId : 0;
            return View();
        }

        // POST: Add Question - Uses form collection to get question details and adds question to the database
        [HttpPost]
        public ActionResult AddQuestion(int assessmentId, string questionType, string questionText, string correctAnswer, string questionAnswer = null)
        {
            try
            {
                Question question = new Question(assessmentId, questionType, questionText, correctAnswer);
                question.questionAnswer = questionAnswer;
                new QuestionDAL().CreateQuestion(question);
                TempData["success"] = "Question added successfully";
                return RedirectToAction("AddQuestion", new { assessmentId });
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                Assessment assessment = new AssessmentDAL().GetAssessmentById(assessmentId);
                ViewBag.AssessmentId = assessmentId;
                ViewBag.LessonId = assessment != null ? assessment.lessonId : 0;
                return View();
            }
        }

        // POST: Delete Assessment - after assessment is deleted return to Lesson page
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index", "Course");

            Assessment assessment = new AssessmentDAL().GetAssessmentById(id);
            new AssessmentDAL().DeleteAssessment(id);
            TempData["success"] = "Assessment deleted successfully";
            return RedirectToAction("Details", "Lesson", new { id = assessment.lessonId });
        }
    }
}