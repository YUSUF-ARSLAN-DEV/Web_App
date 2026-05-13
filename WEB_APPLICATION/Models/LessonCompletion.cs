using System;

namespace WEB_APPLICATION.Models
{
    public class LessonCompletion
    {
        public int completionId { get; set; }
        public int userId { get; set; }
        public int lessonId { get; set; }
        public int courseId { get; set; }
        public DateTime completedAt { get; set; }

        public LessonCompletion(int userId, int lessonId, int courseId) // basically this class is like a book mark of completitin g alesson 
        {
            this.userId = userId;
            this.lessonId = lessonId;
            this.courseId = courseId;
            this.completedAt = DateTime.Now;
        }

        public LessonCompletion(int completionId, int userId, int lessonId, int courseId, DateTime completedAt)
        {
            this.completionId = completionId;
            this.userId = userId;
            this.lessonId = lessonId;
            this.courseId = courseId;
            this.completedAt = completedAt;
        }
    }
}