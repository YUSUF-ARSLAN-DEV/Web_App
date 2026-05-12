using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class ForumController : Controller
    {
        // GET: Forums by Course
        public ActionResult Index(int courseId)
        {
            List<Forum> forums = new ForumDAL().GetForumsByCourse(courseId);
            ViewBag.CourseId = courseId;
            return View(forums);
        }

        // GET: Forum Details
        public ActionResult Details(int id)
        {
            Forum forum = new ForumDAL().GetForumById(id);
            if (forum == null)
                return HttpNotFound();
            List<Post> posts = new PostDAL().GetPostsByForum(id);
            ViewBag.Posts = posts;
            return View(forum);
        }

        // GET: Create Forum (instructor/admin)
        [HttpGet]
        public ActionResult Create(int courseId)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Index", "Course");
            ViewBag.CourseId = courseId;
            return View();
        }

        // POST: Create Forum
        [HttpPost]
        public ActionResult Create(int courseId, string title, string postFlair)
        {
            try
            {
                Forum forum = new Forum(0, courseId, title, postFlair);
                new ForumDAL().CreateForum(forum);
                TempData["success"] = "Forum created successfully";
                return RedirectToAction("Index", new { courseId });
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // POST: Delete Forum
        [HttpPost]
        public ActionResult Delete(int id, int courseId)
        {
            if (Session["role"] == null || (Session["role"].ToString() != "admin" && Session["role"].ToString() != "instructor"))
                return RedirectToAction("Login", "Account");
            new ForumDAL().DeleteForum(id);
            TempData["success"] = "Forum deleted successfully";
            return RedirectToAction("Index", new { courseId });
        }
    }
}