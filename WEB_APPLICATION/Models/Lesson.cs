using System;

namespace WEB_APPLICATION.Models
{
    public class Lesson
    {
        public int lessonId { get; set; }
        public int courseId { get; set; }
        public string lessonTitle { get; set; }
        public string lessonContent { get; set; }

        public Lesson(int lessonId, int courseId, string lessonTitle, string lessonContent)
        {
            this.lessonId = lessonId;
            this.courseId = courseId;
            this.lessonTitle = lessonTitle;
            this.lessonContent = lessonContent;
        }
    }
}
