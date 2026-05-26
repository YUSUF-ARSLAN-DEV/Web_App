using System;
using System.Collections.Generic;
using System.IO;
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

        // This is the updated method that is used to create  a post now it uses the BlobStorage helper class to upload the image to Azure Blob Storage and
        // get the URL of the uploaded image. The URL is then stored in the database along with the post details. 
        // POST: The method used to create a post
        [HttpPost]
        public ActionResult Create(int forumId, string title, string textContent, HttpPostedFileBase imageFile)
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");
            if (string.IsNullOrWhiteSpace(title))
            {
                ViewBag.Error = "Post title is required.";
                return View();
            }
            if (string.IsNullOrWhiteSpace(textContent))
            {
                ViewBag.Error = "Post content is required.";
                return View();
            }
            try
            {
                int userId = (int)Session["userId"];
                string imageUrl = null;
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    string uniqueName = System.Guid.NewGuid().ToString() + "_" + fileName;
                    imageUrl = BlobStorageHelper.UploadFile(imageFile.InputStream, uniqueName, "uploads");
                }
                Post post = new Post(forumId, userId, title, textContent, imageUrl);
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
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
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

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Delete old image from Blob if exists
                    if (!string.IsNullOrEmpty(post.imageUrl))
                    {
                        string oldFileName = Path.GetFileName(post.imageUrl);
                        BlobStorageHelper.DeleteFile(oldFileName, "uploads");
                    }
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    string uniqueName = System.Guid.NewGuid().ToString() + "_" + fileName;
                    post.imageUrl = BlobStorageHelper.UploadFile(imageFile.InputStream, uniqueName, "uploads");
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