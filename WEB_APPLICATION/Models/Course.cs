using System;

namespace WEB_APPLICATION.Models
{
    public class Course
    {
        public int courseId { get; set; }
        public int userId { get; set; }
        public string courseDescription { get; set; }
        public string courseName { get; set; }
        public bool activeStatus { get; set; }
        public string imageUrl { get; set; }

        public Course(int courseId, int userId, string courseName, string courseDescription, string imageUrl)
        {
            this.courseId = courseId;
            this.userId = userId;
            this.courseName = courseName;
            this.courseDescription = courseDescription;
            this.imageUrl = imageUrl;
            this.activeStatus = true;
        }
        // this constructor is for creating a new course - for first time insertion into the data base 
        public Course(int userId , String courseName , String courseDescription , String imageUrl )
        {
            this.userId = userId;
            this.courseName = courseName;
            this.courseDescription = courseDescription;
            this.imageUrl = imageUrl;
            this.activeStatus = true;
        }
        public Course(int courseId, int userId, string courseName, string courseDescription, string imageUrl, bool activeStatus)
        {
            this.courseId = courseId;
            this.userId = userId;
            this.courseName = courseName;
            this.courseDescription = courseDescription;
            this.imageUrl = imageUrl;
            this.activeStatus = activeStatus;
        }
    }
}

//this is to make sure it works
