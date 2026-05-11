namespace WEB_APPLICATION.Models
{
    public class InstructorCourseViewModel // this is a container that is used to load the necessary data that will be displayed in the instructor's course management page. It contains the course details, the number of students enrolled, the average rating, and the number of lessons in the course.
    {
        public Course course { get; set; }
        public int studentCount { get; set; }
        public float avgRating { get; set; }
        public int lessonCount { get; set; }
    }
}