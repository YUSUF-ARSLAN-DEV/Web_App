using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient ; 
using System.Web;

namespace WEB_APPLICATION.Models
{
    public class CourseDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection() ; 
        

        // the method below Creates a new course 
        public bool createCourse(int userId , String courseName , String courseDescription, String imageUrl  )
        {

            bool success = false;
            Course course = new Course(userId , courseName , courseDescription , imageUrl ) ;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Course  (userId, courseDescription  , courseName   ,   activeStatus   ,   imageUrl   ) VALUES (@userId, @courseDescription, @courseName, @activeStatus, @imageUrl)", conn);
                    cmd.Parameters.AddWithValue("@userId", course.userId);
                    cmd.Parameters.AddWithValue("@courseDescription", course.courseDescription);
                    cmd.Parameters.AddWithValue("@courseName", course.courseName);
                    cmd.Parameters.AddWithValue("@activeStatus", course.activeStatus);
                    cmd.Parameters.AddWithValue("@imageUrl", (Object)course.imageUrl ?? DBNull.Value  ) ; 
                    success = true;
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;
        }

        // the method below takes a course ID and deletes the course entry form the data base - through a soft delete not a hard delete 
        public bool deleteCourse(int courseId )
        {
            bool success = false;
            try
            {
                conn.Open();
                using (
                SqlCommand cmd = new SqlCommand(
                "UPDATE Course SET activeStatus= 0  WHERE  courseId = @courseId", conn)) {
                cmd.Parameters.AddWithValue("@courseId", courseId);
                if (cmd.ExecuteNonQuery() > 0)
                    success = true;
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;
        } 
        

        // the method below takes courseName and description and updates these fields using the courseID 
        public bool updateCourse(int courseId, String courseName ,  String courseDescription  )
        {
            bool success = false;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Course SET courseName = @courseName , courseDescription = @courseDescription  WHERE courseId = @courseId ", conn);
                cmd.Parameters.AddWithValue("@courseId", courseId );
                cmd.Parameters.AddWithValue("@courseName", courseName );
                cmd.Parameters.AddWithValue("@courseDescription", courseDescription );
                if (cmd.ExecuteNonQuery() > 0) 
                    success = true;
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;
        }
        

        // The method below takes an a user ID and returns all courses created by that User  - this is an instructor method 
        public List<Course> getCoursesByUserId(int specifiedUserId)
        {
            int courseId ; 
            int userId ; // this here refers to the ID of the instructor who created thsi 
            String courseDescription ; 
            String courseName ; 
            bool activeStatus ; 
            String imageUrl ; 
            List<Course> courseList = new List<Course>();
            try
            {
                conn.Open();
                using ( SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Course  WHERE userId = @specifiedUserId", conn))
                {
                    cmd.Parameters.AddWithValue("@specifiedUserId", specifiedUserId)  ; 
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Course c =  new Course(
                                UtilityDAL.returnInt(reader, "courseId"),
                                UtilityDAL.returnInt(reader, "userId"),
                                UtilityDAL.returnString(reader, "courseName"),
                                UtilityDAL.returnString(reader, "courseDescription"),
                                UtilityDAL.returnString(reader, "imageUrl"),
                                UtilityDAL.returnBit(reader, "activeStatus") 
                            );
                            courseList.Add(c); 
                        }
                    }
                }
                
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return courseList;
        }

        // the Id is passsed to the method and the course object is retrived 
        public Course getCourseById(int courseId )
        {
            Course course = null ; 
            try
            {
                conn.Open();
                using ( SqlCommand cmd = new SqlCommand("SELECT * FROM Course  WHERE courseId = @courseId", conn))      
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId ) ;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // if the reader finds teh value 
                            {
                                course = new Course(
                                    UtilityDAL.returnInt(reader, "courseId"),
                                    UtilityDAL.returnInt(reader, "userId"),
                                    UtilityDAL.returnString(reader, "courseName"),
                                    UtilityDAL.returnString(reader, "courseDescription"),
                                    UtilityDAL.returnString(reader, "imageUrl"),
                                    UtilityDAL.returnBit(reader, "activeStatus")                                 
                                );
                            }
                        }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return course ; 
        }

         









    }
}