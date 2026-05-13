using System;

namespace WEB_APPLICATION.Models
{
    public class Lesson
    {
        public int lessonId { get; set; }
        public int courseId { get; set; }
        public string lessonTitle { get; set; }
        public string lessonContent { get; set; }

        // These properties will allow for file uploads 
        public string videoUrl { get; set; }        
        public string attachmentUrl { get; set; }   
        public string attachmentName { get; set; }  

        // Constructor 1: Full read from database
        public Lesson(int lessonId, int courseId, string lessonTitle, string lessonContent,
                      string videoUrl = null, string attachmentUrl = null, string attachmentName = null)
        {
            this.lessonId = lessonId;
            this.courseId = courseId;
            this.lessonTitle = lessonTitle;
            this.lessonContent = lessonContent;
            this.videoUrl = videoUrl;
            this.attachmentUrl = attachmentUrl;
            this.attachmentName = attachmentName;
        }

        // Constructor 2: For new lesson creation
        public Lesson(int courseId, string lessonTitle, string lessonContent,
                      string videoUrl = null, string attachmentUrl = null, string attachmentName = null)
        {
            this.courseId = courseId;
            this.lessonTitle = lessonTitle;
            this.lessonContent = lessonContent;
            this.videoUrl = videoUrl;
            this.attachmentUrl = attachmentUrl;
            this.attachmentName = attachmentName;
        }
    }
}