using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Management;



namespace WEB_APPLICATION.Models
{
    public class PostDAL
    {
      private SqlConnection conn = UtilityDAL.createConnection(); 

        public  bool CreatePost(Post post)
        {
            bool success = false ; 
            try 
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Post (forumId, userId, title, textContent, imageUrl, postDate, postTime) VALUES (@forumId, @userId, @title, @textContent, @imageUrl, @postDate, @postTime)", conn))
                {
                    cmd.Parameters.AddWithValue("@forumId", post.forumId);
                    cmd.Parameters.AddWithValue("@userId", post.userId);
                    cmd.Parameters.AddWithValue("@title", post.title);
                    cmd.Parameters.AddWithValue("@textContent", post.textContent);
                    // casting ImageUrl to an object as ?? needs both sides to be compatible but if 
                    cmd.Parameters.AddWithValue("@imageUrl", (object)post.imageUrl ?? System.DBNull.Value); // a muhc shorter version of if else 
                    cmd.Parameters.AddWithValue("@postDate", post.postDate);
                    cmd.Parameters.AddWithValue("@postTime", post.postTime);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                success = true ; 
            } 
            catch (SqlException  e ) 
            {
                success = false ; 
                Console.WriteLine(e.Message) ;     
            }
            finally 
            {
                
                conn.Close() ; 
                
            }
             return success ; // after the connection close return the status 
           
        }

        public List<Post> getPostsByForum(int requiredForumId) // this method returns a list of Post objects that belong to a specific forum
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

        public bool updatePost(Post post)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Post SET title = @title, textContent = @textContent, imageUrl = @imageUrl WHERE postId = @postId", conn))
                {
                    cmd.Parameters.AddWithValue("@title", post.title);
                    cmd.Parameters.AddWithValue("@textContent", post.textContent);
                    cmd.Parameters.AddWithValue("@imageUrl", (object)post.imageUrl ?? System.DBNull.Value); // the ?? operator checks for null value 
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

        public bool deletePost(int postId)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Post WHERE postId = @postId", conn))
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
    }
}
