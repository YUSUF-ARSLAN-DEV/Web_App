using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class LessonDAL
    {
        private SqlConnection conn;

        public LessonDAL()
        {
            conn = UtilityDAL.createConnection();
        }

        public bool CreateLesson(Lesson lesson)
        {
            try
            {
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
            catch (SqlException)
            {
                return false;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public List<Lesson> GetLessonsByCourse(int courseId)
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
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return lessons;
        }

        public Lesson GetLessonById(int lessonId)
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
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return null;
        }

        public bool UpdateLesson(Lesson lesson)
        {
            try
            {
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
            catch (SqlException)
            {
                return false;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public bool DeleteLesson(int lessonId)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Lesson WHERE lessonId = @lessonId", conn))
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
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}