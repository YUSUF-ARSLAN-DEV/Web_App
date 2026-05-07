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

        public List<Post> GetPostsByForum(int requiredForumId) // this method returns a list of pots objects that belong to a specific forum 
        {
            List<Post> posts = new List<Post>();
            int postId ;
            int userId ;
            int forumId ;
            string title ;
            string content ;
            string imagePath ;
            DateTime postDate ;
            TimeSpan postTime ;
            using (SqlCommand cmd = new SqlCommand(
                "SELECT postId, forumId, userId, title, textContent, imageUrl, postDate, postTime FROM Post WHERE forumId = @forumId", conn))
            {
                cmd.Parameters.AddWithValue("@forumId", requiredForumId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        postId = UtilityDAL.returnInt(reader, "postId");
                        userId = UtilityDAL.returnInt(reader, "userId");
                        forumId = UtilityDAL.returnInt(reader, "forumId");
                        title = UtilityDAL.returnString(reader, "title");
                        content = UtilityDAL.returnString(reader, "textContent");
                        imagePath = UtilityDAL.returnString(reader, "imageUrl");
                        postDate = UtilityDAL.returnDateTime(reader, "postDate");
                        postTime = UtilityDAL.returnTimeSpan(reader, "postTime");

                        Post post = new Post(postId, forumId, userId, title, content, imagePath, postDate, postTime);

                        posts.Add(post);
                    }
                }
            }
            return posts;
        }

        public void UpdatePost(Post post)
        {

            using (SqlCommand cmd = new SqlCommand(
                "UPDATE Post SET title = @title, textContent = @textContent, imageUrl = @imageUrl WHERE postId = @postId", conn))
            {
                cmd.Parameters.AddWithValue("@title", post.title);
                cmd.Parameters.AddWithValue("@textContent", post.textContent);
                cmd.Parameters.AddWithValue("@imageUrl", (object)post.imageUrl ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@postId", post.postId );
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeletePost(int postId)
        {

            using (SqlCommand cmd = new SqlCommand("DELETE FROM Post WHERE postId = @postId", conn))
            {
                cmd.Parameters.AddWithValue("@postId", postId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
