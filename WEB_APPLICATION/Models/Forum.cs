using System;

namespace WEB_APPLICATION.Models
{
    public class Forum
    {
        public int forumId { get; set; }
        public int courseId { get; set; }
        public string title { get; set; }
        public string postFlair { get; set; }

        public Forum(int forumId, int courseId, string title, string postFlair)
        {
            this.forumId = forumId;
            this.courseId = courseId;
            this.title = title;
            this.postFlair = postFlair;
        }
    }
}