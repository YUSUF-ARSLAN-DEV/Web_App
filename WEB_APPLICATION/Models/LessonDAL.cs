using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class LessonDAL
    {
        public static bool CreateLesson(Lesson lesson)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Lesson (courseId, lessonTitle, lessonContent) VALUES (@courseId, @lessonTitle, @lessonContent)", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", lesson.CourseID);
                    cmd.Parameters.AddWithValue("@lessonTitle", lesson.LessonTitle);
                    cmd.Parameters.AddWithValue("@lessonContent", lesson.LessonContent);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (SqlException e)
            {
                return false;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        public static List<Lesson> GetLessonsByCourse(int courseId)
        {
            List<Lesson> lessons = new List<Lesson>();
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
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
            catch (SqlException e)
            {
                return null;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return lessons;
        }

        public static Lesson GetLessonById(int lessonId)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
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
            catch (SqlException e)
            {
                return null;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return null;
        }

        public static bool UpdateLesson(Lesson lesson)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Lesson SET lessonTitle = @lessonTitle, lessonContent = @lessonContent WHERE lessonId = @lessonId", conn))
                {
                    cmd.Parameters.AddWithValue("@lessonTitle", lesson.LessonTitle);
                    cmd.Parameters.AddWithValue("@lessonContent", lesson.LessonContent);
                    cmd.Parameters.AddWithValue("@lessonId", lesson.LessonID);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (SqlException e)
            {
                return false;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        public static bool DeleteLesson(int lessonId)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Lesson WHERE lessonId = @lessonId", conn))
                {
                    cmd.Parameters.AddWithValue("@lessonId", lessonId);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (SqlException e)
            {
                return false;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }
    }
}
