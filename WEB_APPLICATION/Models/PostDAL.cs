using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class PostDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection() ; 
        public  bool CreatePost(Post post)
        {
            
            try
            {
                
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Post (forumId, userId, title, textContent, imageUrl, postDate, postTime) VALUES (@forumId, @userId, @title, @textContent, @imageUrl, @postDate, @postTime)", conn))
                {
                    cmd.Parameters.AddWithValue("@forumId", post.forumId);
                    cmd.Parameters.AddWithValue("@userId", post.userId);
                    cmd.Parameters.AddWithValue("@title", post.title);
                    cmd.Parameters.AddWithValue("@textContent", post.textContent);
                    cmd.Parameters.AddWithValue("@imageUrl", (object)post.imageUrl ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@postDate", post.postDate);
                    cmd.Parameters.AddWithValue("@postTime", post.postTime);
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

        public  List<Post> GetPostsByForum(int requiredForumId)
        {
            List<Post> posts = new List<Post>();
           
            try
            {
                
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT postId, forumId, userId, title, textContent, imageUrl, postDate, postTime FROM Post WHERE forumId = @forumId", conn))
                {
                    cmd.Parameters.AddWithValue("@forumId", requiredForumId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int postId = UtilityDAL.returnInt(reader, "postId");
                            int userId = UtilityDAL.returnInt(reader, "userId");
                            int forumId = UtilityDAL.returnInt(reader, "forumId");
                            string title = UtilityDAL.returnString(reader, "title");
                            string content = UtilityDAL.returnString(reader, "textContent");
                            string imagePath = UtilityDAL.returnString(reader, "imageUrl");
                            DateTime postDate = UtilityDAL.returnDateTime(reader, "postDate");
                            TimeSpan postTime = UtilityDAL.returnTimeSpan(reader, "postTime");
                            Post post = new Post(postId, forumId, userId, title, content, imagePath, postDate, postTime);
                            posts.Add(post);
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
            return posts;
        }

        public  bool UpdatePost(Post post)
        {
            
            try
            {
                
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Post SET title = @title, textContent = @textContent, imageUrl = @imageUrl WHERE postId = @postId", conn))
                {
                    cmd.Parameters.AddWithValue("@title", post.title);
                    cmd.Parameters.AddWithValue("@textContent", post.textContent);
                    cmd.Parameters.AddWithValue("@imageUrl", (object)post.imageUrl ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@postId", post.postId);
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

        public  bool DeletePost(int postId)
        {
           
            try
            {
                
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Post WHERE postId = @postId", conn))
                {
                    cmd.Parameters.AddWithValue("@postId", postId);
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

        public  Post GetPostById(int postId)
        {
            
            try
            {
                
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT postId, forumId, userId, title, textContent, imageUrl, postDate, postTime FROM Post WHERE postId = @postId", conn))
                {
                    cmd.Parameters.AddWithValue("@postId", postId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int pId = UtilityDAL.returnInt(reader, "postId");
                            int fId = UtilityDAL.returnInt(reader, "forumId");
                            int uId = UtilityDAL.returnInt(reader, "userId");
                            string title = UtilityDAL.returnString(reader, "title");
                            string content = UtilityDAL.returnString(reader, "textContent");
                            string imageUrl = UtilityDAL.returnString(reader, "imageUrl");
                            DateTime postDate = UtilityDAL.returnDateTime(reader, "postDate");
                            TimeSpan postTime = UtilityDAL.returnTimeSpan(reader, "postTime");
                            return new Post(pId, fId, uId, title, content, imageUrl, postDate, postTime);
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
    }
}
            