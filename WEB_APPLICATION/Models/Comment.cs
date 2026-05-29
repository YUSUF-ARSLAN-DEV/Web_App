using System;

namespace WEB_APPLICATION.Models
{
    public class Comment
    {
        public int commentId { get; set; }
        public int postId { get; set; }
        public int userId { get; set; }
        public string commentText { get; set; }
        public DateTime commentDate { get; set; }
        public TimeSpan commentTime { get; set; }

        // Constructor for creating a new comment
        public Comment(int postId, int userId, string commentText)
        {
            this.postId = postId;
            this.userId = userId;
            this.commentText = commentText;
            this.commentDate = DateTime.Now.Date;
            this.commentTime = DateTime.Now.TimeOfDay;
        }

        // Constructor for reading from database
        public Comment(int commentId, int postId, int userId, string commentText, DateTime commentDate, TimeSpan commentTime)
        {
            this.commentId = commentId;
            this.postId = postId;
            this.userId = userId;
            this.commentText = commentText;
            this.commentDate = commentDate;
            this.commentTime = commentTime;
        }
    }
}