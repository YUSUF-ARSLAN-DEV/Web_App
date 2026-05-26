using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class RatingDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection();

        // The method that allows the students to add their ratings and comments for the courses they have taken 
        public bool AddRating(int courseId, int userId, int score, string comment)
        {
            bool success = false;
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Rating (courseId, userId, score, comment, ratingDate) VALUES (@courseId, @userId, @score, @comment, @ratingDate)", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@score", score);
                    cmd.Parameters.AddWithValue("@comment", (object)comment ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ratingDate", DateTime.Now);
                    if (cmd.ExecuteNonQuery() > 0)
                        success = true;
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;
        }

        public List<Rating> GetRatingsByCourse(int courseId)
        {
            List<Rating> ratings = new List<Rating>();
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Rating WHERE courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ratings.Add(new Rating(
                                UtilityDAL.returnInt(reader, "ratingId"),
                                UtilityDAL.returnInt(reader, "courseId"),
                                UtilityDAL.returnInt(reader, "userId"),
                                UtilityDAL.returnInt(reader, "score"),
                                UtilityDAL.returnString(reader, "comment"),
                                UtilityDAL.returnDateTime(reader, "ratingDate")
                            ));
                        }
                    }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return ratings;
        }

        public float GetAverageRating(int courseId)
        {
            float average = 0f;
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT AVG(CAST(score AS FLOAT)) FROM Rating WHERE courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        average = Convert.ToSingle(result);
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return average;
        }

        // This method is the logical checks that ensures that the student has not actually rated the course before
        // since duplicate ratings from the same student for the same course should not be allowed 
        public bool HasUserRated(int userId, int courseId)
        {
            int count = 0;
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Rating WHERE userId = @userId AND courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        count = Convert.ToInt32(result);
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return count > 0;
        }
    }
}