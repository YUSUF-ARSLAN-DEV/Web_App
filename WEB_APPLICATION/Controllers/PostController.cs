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
        public ActionResult Create(int forumId, string title, string textContent, HttpPostedFileBase imageFile)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                int userId = (int)Session["userId"];
                string imageUrl = null;

                // DEBUG: Check if file was received
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    TempData["uploadDebug"] = $"File received: {imageFile.FileName}, Size: {imageFile.ContentLength}";

                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    string uniqueName = System.Guid.NewGuid().ToString() + "_" + fileName;
                    string savePath = Server.MapPath("~/Uploads/" + uniqueName);
                    imageFile.SaveAs(savePath);
                    imageUrl = "/Uploads/" + uniqueName;
                }
                else
                {
                    TempData["uploadDebug"] = "No file received - imageFile is null";
                }

                Post post = new Post(0, forumId, userId, title, textContent, imageUrl);
                bool success = new PostDAL().CreatePost(post);

                if (success)
                {
                    TempData["success"] = "Post created successfully";
                    return RedirectToAction("Details", "Forum", new { id = forumId });
                }

                ViewBag.Error = "Failed to create post";
                return View();
            }
            catch (Exception ex)
            {
                TempData["uploadDebug"] = $"Error: {ex.Message}";
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
            Post post = new PostDAL().GetPostById(id);
            if (post == null)
                return HttpNotFound();
            if (post.userId != (int)Session["userId"])
                return new HttpUnauthorizedResult();
            return View(post);
        }

        // POST: Edit Post (with image upload)
        [HttpPost]
        public ActionResult Edit(int postId, int forumId, string title, string textContent, HttpPostedFileBase imageFile)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                Post post = new PostDAL().GetPostById(postId);
                if (post == null)
                    return HttpNotFound();

                post.title = title;
                post.textContent = textContent;

                // Handle image upload if a new file is provided
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(post.imageUrl))
                    {
                        string oldPath = Server.MapPath(post.imageUrl);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    // Save new image
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    string uniqueName = System.Guid.NewGuid().ToString() + "_" + fileName;
                    string savePath = Server.MapPath("~/Uploads/" + uniqueName);
                    imageFile.SaveAs(savePath);
                    post.imageUrl = "/Uploads/" + uniqueName;
                }

                new PostDAL().UpdatePost(post);
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
        public ActionResult Delete(int id, int forumId)
        {
            System.Diagnostics.Debug.WriteLine($"DELETE CALLED - id: {id}, forumId: {forumId}");
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            string role = Session["role"].ToString();
            int currentUserId = (int)Session["userId"];
            Post post = new PostDAL().GetPostById(id);

            if (post == null)
            {
                TempData["error"] = "Post not found";
                return RedirectToAction("Details", "Forum", new { id = forumId });
            }

            // Get the forum to check instructor ownership
            Forum forum = new ForumDAL().GetForumById(forumId);

            bool isOwner = (post.userId == currentUserId);
            bool isAdmin = (role == "admin");
            bool isInstructorOfThisForum = (role == "instructor" && forum != null && forum.courseId == new CourseDAL().GetCourseById(forum.courseId).userId);

            if (!isOwner && !isAdmin && !isInstructorOfThisForum)
            {
                TempData["error"] = "You don't have permission to delete this post";
                return RedirectToAction("Details", "Forum", new { id = forumId });
            }

            // Delete image file if exists
            if (!string.IsNullOrEmpty(post.imageUrl))
            {
                string filePath = Server.MapPath(post.imageUrl);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            new PostDAL().DeletePost(id);
            TempData["success"] = "Post deleted successfully";
            return RedirectToAction("Details", "Forum", new { id = forumId });
        }
    }
}