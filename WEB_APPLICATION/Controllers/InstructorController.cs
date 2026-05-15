using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class InstructorController : Controller
    {
        public ActionResult Dashboard()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["role"].ToString() != "instructor")
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["userId"];

            List<Course> courses = new CourseDAL().GetCoursesByUserId(userId).FindAll(c => c.activeStatus);
            List<InstructorCourseViewModel> dashboardData = new List<InstructorCourseViewModel>();

            foreach (Course course in courses)
            {
                List<EnrollmentRecord> enrollments = new EnrollmentDAL().GetEnrollmentByCourse(course.courseId);
                int studentCount = enrollments != null ? enrollments.Count : 0;
                float avgRating = new RatingDAL().GetAverageRating(course.courseId);
                int lessonCount = new LessonDAL().GetLessonCountByCourse(course.courseId);

                dashboardData.Add(new InstructorCourseViewModel
                {
                    course = course,
                    studentCount = studentCount,
                    avgRating = avgRating,
                    lessonCount = lessonCount
                });
            }

            return View(dashboardData);
        }
    }
}