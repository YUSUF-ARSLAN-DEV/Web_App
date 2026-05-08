using System;

namespace WEB_APPLICATION.Models
{
    public class Post
    {
        public int postId { get; set; }
        public int forumId { get; set; }
        public int userId { get; set; }
        public string title { get; set; }
        public string textContent { get; set; }
        public string imageUrl { get; set; }
        public DateTime postDate { get; set; }
        public TimeSpan  postTime { get; set; }
        // used for creation 
        public Post(int postId, int forumId , int userId, string title, string textContent, string imageUrl)
        {
            this.postId = postId;
            this.forumId = forumId;
            this.userId = userId;
            this.title = title;
            this.textContent = textContent;
            this.imageUrl = imageUrl;
            this.postDate = DateTime.Now.Date ; // returns only the date to match SQLs date 
            this.postTime = DateTime.Now.TimeOfDay; // returns a TimeSpan value 
        }
        // overloaded construction for reading 
        public Post(int postId, int forumId, int userId, string title, string textContent, string imageUrl , DateTime postDate , TimeSpan postTime ) 
        {
            this.postId = postId;
            this.forumId = forumId;
            this.userId = userId;
            this.title = title;
            this.textContent = textContent;
            this.imageUrl = imageUrl;
            this.postDate = postDate  ;  
            this.postTime = postTime ; 
        }
    
    }
}
