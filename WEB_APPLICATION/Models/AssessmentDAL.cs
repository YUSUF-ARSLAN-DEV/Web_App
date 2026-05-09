using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class AssessmentDAL
    {
        public static bool CreateAssessment(Assessment assessment)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Assessment (lessonId, attemptNumber) VALUES (@lessonId, @attemptNumber)", conn))
                {
                    cmd.Parameters.AddWithValue("@lessonId", assessment.LessonID);
                    cmd.Parameters.AddWithValue("@attemptNumber", assessment.AttemptNumber);
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

        public static List<Assessment> GetAssessmentsByLesson(int lessonId)
        {
            List<Assessment> assessments = new List<Assessment>();
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT assessmentId, lessonId, attemptNumber FROM Assessment WHERE lessonId = @lessonId", conn))
                {
                    cmd.Parameters.AddWithValue("@lessonId", lessonId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int assessmentId = UtilityDAL.returnInt(reader, "assessmentId");
                            int lessonID = UtilityDAL.returnInt(reader, "lessonId");
                            int attemptNumber = UtilityDAL.returnInt(reader, "attemptNumber");
                            Assessment a = new Assessment(assessmentId, lessonID);
                            a.AttemptNumber = attemptNumber;
                            assessments.Add(a);
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
            return assessments;
        }

        public static Assessment GetAssessmentById(int assessmentId)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT assessmentId, lessonId, attemptNumber FROM Assessment WHERE assessmentId = @assessmentId", conn))
                {
                    cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int aId = UtilityDAL.returnInt(reader, "assessmentId");
                            int lessonId = UtilityDAL.returnInt(reader, "lessonId");
                            int attemptNumber = UtilityDAL.returnInt(reader, "attemptNumber");
                            Assessment a = new Assessment(aId, lessonId);
                            a.AttemptNumber = attemptNumber;
                            return a;
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

        public static bool DeleteAssessment(int assessmentId)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Assessment WHERE assessmentId = @assessmentId", conn))
                {
                    cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
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
        public static bool IncrementAttempt(int assessmentId)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Assessment SET attemptNumber = attemptNumber + 1 WHERE assessmentId = @assessmentId", conn))
                {
                    cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
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
