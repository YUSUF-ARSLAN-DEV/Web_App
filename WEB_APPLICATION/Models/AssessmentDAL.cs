using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class AssessmentDAL
    {
        SqlConnection conn  = UtilityDAL.createConnection() ; 
        public  bool CreateAssessment(Assessment assessment)
        {
            
            try
            {
                
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Assessment (lessonId, attemptNumber) VALUES (@lessonId, @attemptNumber)", conn))
                {
                    cmd.Parameters.AddWithValue("@lessonId", assessment.lessonId);
                    cmd.Parameters.AddWithValue("@attemptNumber", assessment.attemptNumber);
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
                 conn.Close();
            }
        }

        public  List<Assessment> GetAssessmentsByLesson(int lessonId)
        {
            List<Assessment> assessments = new List<Assessment>();
            
            try
            {
                
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
                            a.attemptNumber = attemptNumber;
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
                 conn.Close();
            }
            return assessments;
        }

        public  Assessment GetAssessmentById(int assessmentId)
        {
           
            try
            {
               
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
                            a.attemptNumber = attemptNumber;
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
                 conn.Close();
            }
            return null;
        }

        public  bool DeleteAssessment(int assessmentId)
        {
            
            try
            {
                
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
                 conn.Close();
            }
        }
        public  bool IncrementAttempt(int assessmentId)
        {
            
            try
            {
                
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
                     conn.Close();
            }
        }
    }
}
