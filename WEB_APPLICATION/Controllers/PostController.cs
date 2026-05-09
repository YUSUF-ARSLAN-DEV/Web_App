using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class PostController : Controller
    {
        // GET: Create Post
        [HttpGet]
        public ActionResult Create(int forumId)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            ViewBag.ForumId = forumId;
            return View();
        }

        // POST: Create Post
        [HttpPost]
        public ActionResult Create(int forumId, string title, string textContent, string imageUrl)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            
            try
            {
                int userId = (int)Session["userId"];
                Post post = new Post(0, forumId, userId, title, textContent, imageUrl);
                bool success = PostDAL.CreatePost(post);
                if (success)
                {
                    TempData["success"] = "Post created successfully";
                    return RedirectToAction("Details", "Forum", new { id = forumId });
                }
                ViewBag.Error = "Failed to create post";
                return View();
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // GET: Edit Post
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            Post post = PostDAL.GetPostById(id);
            if (post == null)
                return HttpNotFound();
            if (post.userId != (int)Session["userId"])
                return new HttpUnauthorizedResult();
            return View(post);
        }

        // POST: Edit Post
        [HttpPost]
        public ActionResult Edit(int postId, int forumId, string title, string textContent, string imageUrl)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            
            try
            {
                Post post = new Post(postId, forumId, (int)Session["userId"], title, textContent, imageUrl);
                PostDAL.UpdatePost(post);
                TempData["success"] = "Post updated successfully";
                return RedirectToAction("Details", "Forum", new { id = forumId });
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
        }

        // POST: Delete Post
        [HttpPost]
        public ActionResult Delete(int id, int forumId)
        {
            if (Session["userId"] == null)
                return Json(new { success = false });
            Post post = PostDAL.GetPostById(id);
            if (post == null || post.userId != (int)Session["userId"])
                return Json(new { success = false });
            PostDAL.DeletePost(id);
            TempData["success"] = "Post deleted successfully";
            return RedirectToAction("Details", "Forum", new { id = forumId });
        }
    }
}