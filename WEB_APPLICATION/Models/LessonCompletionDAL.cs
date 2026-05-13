using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class LessonCompletionDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection();

        public bool MarkComplete(int userId, int lessonId, int courseId)
        {
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "IF NOT EXISTS (SELECT 1 FROM LessonCompletion WHERE userId = @userId AND lessonId = @lessonId) " +
                    "INSERT INTO LessonCompletion (userId, lessonId, courseId, completedAt) " +
                    "VALUES (@userId, @lessonId, @courseId, @completedAt)", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@lessonId", lessonId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    cmd.Parameters.AddWithValue("@completedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); return false; }
            finally { conn.Close(); }
        }

        public bool IsCompleted(int userId, int lessonId)
        {
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM LessonCompletion WHERE userId = @userId AND lessonId = @lessonId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@lessonId", lessonId);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); return false; }
            finally { conn.Close(); }
        }

        public int GetCompletedCount(int userId, int courseId)
        {
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM LessonCompletion WHERE userId = @userId AND courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); return 0; }
            finally { conn.Close(); }
        }

        public List<int> GetCompletedLessonIds(int userId, int courseId)
        {
            List<int> ids = new List<int>();
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT lessonId FROM LessonCompletion WHERE userId = @userId AND courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            ids.Add(reader.GetInt32(0));
                    }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return ids;
        }
    }
}