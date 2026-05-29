using System;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class CommentController : Controller
    {
        [HttpGet]
        public ActionResult PostCommentSpace(int postId)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            PostDAL postDAL = new PostDAL();
            Post post = postDAL.GetPostById(postId);
            if (post == null)
                return HttpNotFound();

            ViewBag.Post = post;
            ViewBag.Comments = new CommentDAL().GetCommentsByPost(postId);
            ViewBag.ForumId = post.forumId;

            return View("~/Views/Post/PostCommentSpace.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment(int postId, string commentText)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(commentText))
            {
                TempData["error"] = "Comment cannot be empty.";
                return RedirectToAction("PostCommentSpace", new { postId = postId });
            }

            int userId = (int)Session["userId"];
            Comment comment = new Comment(postId, userId, commentText);
            bool success = new CommentDAL().CreateComment(comment);

            if (success)
                TempData["success"] = "Comment added successfully.";
            else
                TempData["error"] = "Failed to add comment.";

            return RedirectToAction("PostCommentSpace", new { postId = postId });
        }
    }
}