using System;

namespace WEB_APPLICATION.Models
{
    public class EnrollmentRecord
    {
        public int enrollmentId { get; set; }
        public int courseId { get; set; }
        public int userId { get; set; }
        public float completionRate { get; set; }
        public DateTime enrollmentDate { get; set; }
        public bool activeStatus { get; set; }

        // constructor to create enrollment records 
        public EnrollmentRecord(int courseId, int userId)
        {
            this.courseId = courseId;
            this.userId = userId;
            this.completionRate = 0;
            this.enrollmentDate = DateTime.Now;
            this.activeStatus = true;
        }
    // Constructor for reading an existing enrollment from the database
    public EnrollmentRecord(int enrollmentId, int courseId, int userId, float completionRate, DateTime enrollmentDate, bool activeStatus)
    {
        this.enrollmentId = enrollmentId;
        this.courseId = courseId;
        this.userId = userId;
        this.completionRate = completionRate;
        this.enrollmentDate = enrollmentDate;
        this.activeStatus = activeStatus;
    }
    }
}
