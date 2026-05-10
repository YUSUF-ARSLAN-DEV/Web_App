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
            Assessment assessment = new AssessmentDAL().GetAssessmentById(assessmentId);
            if (assessment == null)
                return HttpNotFound();
            List<Question> questions = new QuestionDAL().GetQuestionsByAssessment(assessmentId);
            ViewBag.Questions = questions;
            return View(assessment);
        }

        // POST: Submit Quiz
        [HttpPost]
        public ActionResult  SubmitQuiz(int assessmentId, FormCollection answers) 
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
                if (submitted != null && submitted.Trim().Equals(q.correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    correct++;
            }

            new AssessmentDAL().IncrementAttempt(assessmentId);

            TempData["score"] = correct;
            TempData["total"] = total;

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
                
                ViewBag.Score = TempData["score"]; // storing the score of the student 
                ViewBag.Total = TempData["total"];
                
                return View(assessment);
            }

        // GET: Create Assessment (instructor/admin)
        [HttpGet]
        public ActionResult Create(int lessonId)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index");
            ViewBag.LessonId = lessonId;
            return View();
        }

        // POST: Create Assessment
        [HttpPost]
        [ActionName("Create")]
        public ActionResult CreatePost(int lessonId)
        {
            try
            {
                Assessment assessment = new Assessment(0, lessonId);
                bool success = new AssessmentDAL().CreateAssessment(assessment);
                if (success)
                {
                    TempData["success"] = "Assessment created successfully";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Failed to create assessment";
                return View();
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // GET: Add Question
        [HttpGet]
        public ActionResult AddQuestion(int assessmentId)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index");
            ViewBag.AssessmentId = assessmentId;
            return View();
        }

        // POST: Add Question
        [HttpPost]
        public ActionResult AddQuestion(int assessmentId, string questionType, string questionText, string correctAnswer)
        {
            try
            {
                Question question = new Question(0, assessmentId, questionType, questionText, correctAnswer);
                new QuestionDAL().CreateQuestion(question);
                TempData["success"] = "Question added successfully";
                return RedirectToAction("AddQuestion", new { assessmentId });
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // POST: Delete Assessment
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return Json(new { success = false });
            bool success = new AssessmentDAL().DeleteAssessment(id);
            if (success)
                TempData["success"] = "Assessment deleted successfully";
            return RedirectToAction("Index");
        }
    }
}