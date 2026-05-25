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
        // the method below Creates a new course and creates its document folder
        public bool CreateCourse(int userId, string courseName, string courseDescription, string imageUrl)
        {
            bool success = true;
            int newCourseId = 0;
            Course course = new Course(userId, courseName, courseDescription, imageUrl);
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Course (userId, courseDescription, courseName, activeStatus, imageUrl) VALUES (@userId, @courseDescription, " +
                    "@courseName, @activeStatus, @imageUrl); SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", course.userId);
                    cmd.Parameters.AddWithValue("@courseDescription", course.courseDescription);
                    cmd.Parameters.AddWithValue("@courseName", course.courseName);
                    cmd.Parameters.AddWithValue("@activeStatus", course.activeStatus);
                    cmd.Parameters.AddWithValue("@imageUrl", (object)course.imageUrl ?? DBNull.Value);

                    newCourseId = Convert.ToInt32(cmd.ExecuteScalar());

                    // Create the physical folder for this course
                    string courseFolderPath = System.Web.HttpContext.Current.Server.MapPath($"~/DOCUMENTS/Course_{newCourseId}/");
                    if (!System.IO.Directory.Exists(courseFolderPath))
                    {
                        System.IO.Directory.CreateDirectory(courseFolderPath);
                    }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); success = false; }
            finally { conn.Close(); }

            if (newCourseId == 0) { success = false; }
            return success;
        }

        // the method below takes a course ID and deletes the course entry form the data base - through a soft delete not a hard delete 
        public bool DeleteCourse(int courseId )
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
        public bool UpdateCourse(int courseId, string courseName, string courseDescription)
        {
            bool success = false;
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Course SET courseName = @courseName, courseDescription = @courseDescription WHERE courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    cmd.Parameters.AddWithValue("@courseName", courseName);
                    cmd.Parameters.AddWithValue("@courseDescription", courseDescription);
                    if (cmd.ExecuteNonQuery() > 0)
                        success = true;
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;
        }


        // The method below takes an a user ID and returns all courses created by that User  - this is an instructor method 
        public List<Course> GetCoursesByUserId(int specifiedUserId)
        {
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
        public Course GetCourseById(int courseId )
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

        // the method below in essence retrives every single course Object in the  Data base active and 
        // non active as the getAllCourse() method is nan admin  method 
         public List<Course>   GetAllCourses()
        {
            List<Course> courseList = new List<Course>();
            try
            {
                conn.Open();
                using ( SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Course ", conn))
                {

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
        

        // The method below retrives all courses where the passsed filtering word 
        // exiss in the courses name or description 

        public List<Course>  FilterCourses(String filterText )
        {
            List<Course > list = new List<Course >();
            try
            {
                conn.Open();
                using ( SqlCommand cmd = new SqlCommand( // this does not mean duplicate objects but expands the conditoins for an object to be accepted 
                    "SELECT * FROM Course  WHERE activeStatus = 1 AND (courseName LIKE @filterText OR courseDescription LIKE @filterText)   "  , conn) )
                {
                        
                    cmd.Parameters.AddWithValue("@filterText", "%"+filterText+"%"); //so that the final query will look like this "%sorting value%"
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) // while there are rows that exist and the reader moved to 
                        {
                            list.Add(new Course( // creating a course object for the found entry 
                                UtilityDAL.returnInt(reader, "courseId"),
                                UtilityDAL.returnInt(reader, "userId"),
                                UtilityDAL.returnString(reader, "courseName"),
                                UtilityDAL.returnString(reader, "courseDescription"),
                                UtilityDAL.returnString(reader, "imageUrl"),
                                UtilityDAL.returnBit(reader, "activeStatus") 
                            ));
                        }
                    }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }   
            return list ;  
        }
    
        // Reactivate a soft-deleted course - when this method executes in essence the Admin won't see it as an active course and the student won't even find it int he public page where all courses are displayed 
        public bool ReactivateCourse(int courseId)
        {
            bool success = false;
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Course SET activeStatus = 1 WHERE courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    if (cmd.ExecuteNonQuery() > 0)
                        success = true;
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;
        }

        // Get all archived (soft-deleted) courses by instructor
        public List<Course> GetDeletedCoursesByUserId(int userId)
        {
            List<Course> courseList = new List<Course>();
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Course WHERE userId = @userId AND activeStatus = 0", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courseList.Add(new Course(
                                UtilityDAL.returnInt(reader, "courseId"),
                                UtilityDAL.returnInt(reader, "userId"),
                                UtilityDAL.returnString(reader, "courseName"),
                                UtilityDAL.returnString(reader, "courseDescription"),
                                UtilityDAL.returnString(reader, "imageUrl"),
                                UtilityDAL.returnBit(reader, "activeStatus")
                            ));
                        }
                    }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return courseList;
        }

        // the method bleow will be used to get All courses for the public catalogue of course
        public List<Course> GetAllActiveCourses()
        {
            List<Course> courseList = new List<Course>();
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Course WHERE activeStatus = 1", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courseList.Add(new Course(
                                UtilityDAL.returnInt(reader, "courseId"),
                                UtilityDAL.returnInt(reader, "userId"),
                                UtilityDAL.returnString(reader, "courseName"),
                                UtilityDAL.returnString(reader, "courseDescription"),
                                UtilityDAL.returnString(reader, "imageUrl"),
                                UtilityDAL.returnBit(reader, "activeStatus")
                            ));
                        }
                    }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return courseList;
        }
    
    
    }








    
}