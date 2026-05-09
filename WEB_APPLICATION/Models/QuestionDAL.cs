using System.Collections.Generic;
using System.Data.SqlClient;
using System ; 

namespace WEB_APPLICATION.Models
{
    public class QuestionDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection() ; 
        public bool CreateQuestion(Question question)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Question (assessmentId, questionType, questionText, correctAnswer, questionAnswer) VALUES (@assessmentId, @questionType, @questionText, @correctAnswer, @questionAnswer)", conn))
                {
                    cmd.Parameters.AddWithValue("@assessmentId", question.AssessmentID);
                    cmd.Parameters.AddWithValue("@questionType", question.QuestionType);
                    cmd.Parameters.AddWithValue("@questionText", question.QuestionText);
                    cmd.Parameters.AddWithValue("@correctAnswer", question.CorrectAnswer);
                    cmd.Parameters.AddWithValue("@questionAnswer", (object)question.QuestionAnswer ?? System.DBNull.Value);

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

        public List<Question> GetQuestionsByAssessment(int assessmentId)
        {
            List<Question> questions = new List<Question>();

            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT questionId, assessmentId, questionType, questionText, correctAnswer, questionAnswer FROM Question WHERE assessmentId = @assessmentId", conn))
                {
                    cmd.Parameters.AddWithValue("@assessmentId", assessmentId);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int questionId = UtilityDAL.returnInt(reader, "questionId");
                            int aId = UtilityDAL.returnInt(reader, "assessmentId");
                            string questionType = UtilityDAL.returnString(reader, "questionType");
                            string questionText = UtilityDAL.returnString(reader, "questionText");
                            string correctAnswer = UtilityDAL.returnString(reader, "correctAnswer");
                            string questionAnswer = UtilityDAL.returnString(reader, "questionAnswer");

                            Question question = new Question(
                                questionId,
                                aId,
                                questionType,
                                questionText,
                                correctAnswer
                            );

                            question.QuestionAnswer = questionAnswer;

                            questions.Add(question);
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

            return questions;
        }

        public bool UpdateQuestion(Question question)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Question SET questionType = @questionType, questionText = @questionText, correctAnswer = @correctAnswer, questionAnswer = @questionAnswer WHERE questionId = @questionId", conn))
                {
                    cmd.Parameters.AddWithValue("@questionType", question.QuestionType);
                    cmd.Parameters.AddWithValue("@questionText", question.QuestionText);
                    cmd.Parameters.AddWithValue("@correctAnswer", question.CorrectAnswer);
                    cmd.Parameters.AddWithValue("@questionAnswer", (object)question.QuestionAnswer ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@questionId", question.QuestionID);

                    conn.Open();

                    int rows = cmd.ExecuteNonQuery();

                    return rows > 0; // reutrn true if the question row got updated 
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

        public bool deleteQuestion(int questionId)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Question WHERE questionId = @questionId", conn))
                {
                    cmd.Parameters.AddWithValue("@questionId", questionId);

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

        public bool checkAnswer(int questionId, string answer)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT correctAnswer FROM Question WHERE questionId = @questionId", conn))
                {
                    cmd.Parameters.AddWithValue("@questionId", questionId);

                    conn.Open();

                    object result = cmd.ExecuteScalar();

                    return result != null &&
                        result.ToString().Trim().Equals(answer.Trim(), StringComparison.OrdinalIgnoreCase);
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
    }
}
