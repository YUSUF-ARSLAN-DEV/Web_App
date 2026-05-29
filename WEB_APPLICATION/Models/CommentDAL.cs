using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class CommentDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection();

        public bool CreateComment(Comment comment)
        {
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Comment (postId, userId, commentText, commentDate, commentTime) VALUES (@postId, @userId, @commentText, @commentDate, @commentTime)", conn))
                {
                    cmd.Parameters.AddWithValue("@postId", comment.postId);
                    cmd.Parameters.AddWithValue("@userId", comment.userId);
                    cmd.Parameters.AddWithValue("@commentText", comment.commentText);
                    cmd.Parameters.AddWithValue("@commentDate", comment.commentDate);
                    cmd.Parameters.AddWithValue("@commentTime", comment.commentTime);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public List<Comment> GetCommentsByPost(int postId)
        {
            List<Comment> comments = new List<Comment>();
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT commentId, postId, userId, commentText, commentDate, commentTime FROM Comment WHERE postId = @postId ORDER BY commentDate DESC, commentTime DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@postId", postId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new Comment(
                                UtilityDAL.returnInt(reader, "commentId"),
                                UtilityDAL.returnInt(reader, "postId"),
                                UtilityDAL.returnInt(reader, "userId"),
                                UtilityDAL.returnString(reader, "commentText"),
                                UtilityDAL.returnDateTime(reader, "commentDate"),
                                UtilityDAL.returnTimeSpan(reader, "commentTime")
                            ));
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                conn.Close();
            }
            return comments;
        }
    }
}