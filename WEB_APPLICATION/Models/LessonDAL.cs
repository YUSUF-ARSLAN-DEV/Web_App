using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class LessonDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection() ; 
        public  bool CreateLesson(Lesson lesson)
        {
          
            try
            {
             
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Lesson (courseId, lessonTitle, lessonContent) VALUES (@courseId, @lessonTitle, @lessonContent)", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", lesson.courseId);
                    cmd.Parameters.AddWithValue("@lessonTitle", lesson.lessonTitle);
                    cmd.Parameters.AddWithValue("@lessonContent", lesson.lessonContent);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (SqlException)
            {
                return false;
            }
            finally
            {
                 conn.Close();
            }
        }

        public  List<Lesson> GetLessonsByCourse(int courseId)
        {
            List<Lesson> lessons = new List<Lesson>();
            
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT lessonId, courseId, lessonTitle, lessonContent FROM Lesson WHERE courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int lessonId = UtilityDAL.returnInt(reader, "lessonId");
                            int cId = UtilityDAL.returnInt(reader, "courseId");
                            string lessonTitle = UtilityDAL.returnString(reader, "lessonTitle");
                            string lessonContent = UtilityDAL.returnString(reader, "lessonContent");
                            lessons.Add(new Lesson(lessonId, cId, lessonTitle, lessonContent));
                        }
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }
            finally
            {
                 conn.Close();
            }
            return lessons;
        }

        public  Lesson GetLessonById(int lessonId)
        {
            
            try
            {
                
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT lessonId, courseId, lessonTitle, lessonContent FROM Lesson WHERE lessonId = @lessonId", conn))
                {
                    cmd.Parameters.AddWithValue("@lessonId", lessonId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int lId = UtilityDAL.returnInt(reader, "lessonId");
                            int courseId = UtilityDAL.returnInt(reader, "courseId");
                            string lessonTitle = UtilityDAL.returnString(reader, "lessonTitle");
                            string lessonContent = UtilityDAL.returnString(reader, "lessonContent");
                            return new Lesson(lId, courseId, lessonTitle, lessonContent);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }
            finally
            {
                 conn.Close();
            }
            return null;
        }

        public  bool UpdateLesson(Lesson lesson)
        {
           
            try
            {
                
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Lesson SET lessonTitle = @lessonTitle, lessonContent = @lessonContent WHERE lessonId = @lessonId", conn))
                {
                    cmd.Parameters.AddWithValue("@lessonTitle", lesson.lessonTitle);
                    cmd.Parameters.AddWithValue("@lessonContent", lesson.lessonContent);
                    cmd.Parameters.AddWithValue("@lessonId", lesson.lessonId);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (SqlException)
            {
                return false;
            }
            finally
            {
                    conn.Close();
            }
        }

        public  bool DeleteLesson(int lessonId)
        {
            
            try
            {
              
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Lesson WHERE lessonId = @lessonId", conn))
                {
                    cmd.Parameters.AddWithValue("@lessonId", lessonId);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (SqlException)
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public  int GetLessonCountByCourse(int courseId)
        {
            
            try
            {
                
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Lesson WHERE courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count;
                }
            }
            catch (SqlException)
            {
                return 0;
            }
            finally
            {
                 conn.Close();
            }
        }
    }
}
              