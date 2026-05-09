using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class ForumDAL
    {
        public static bool CreateForum(Forum forum)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Forum (courseId, title, postFlair) VALUES (@courseId, @title, @postFlair)", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", forum.courseId);
                    cmd.Parameters.AddWithValue("@title", forum.title);
                    cmd.Parameters.AddWithValue("@postFlair", forum.postFlair);
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
                if (conn != null) conn.Close();
            }
        }

        public static List<Forum> GetForumsByCourse(int courseId)
        {
            List<Forum> forums = new List<Forum>();
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT forumId, courseId, title, postFlair FROM Forum WHERE courseId = @courseId", conn))
                {
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int forumId = UtilityDAL.returnInt(reader, "forumId");
                            int cId = UtilityDAL.returnInt(reader, "courseId");
                            string title = UtilityDAL.returnString(reader, "title");
                            string postFlair = UtilityDAL.returnString(reader, "postFlair");
                            Forum forum = new Forum(forumId, cId, title, postFlair);
                            forums.Add(forum);
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
                if (conn != null) conn.Close();
            }
            return forums;
        }

        public static bool DeleteForum(int forumId)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Forum WHERE forumId = @forumId", conn))
                {
                    cmd.Parameters.AddWithValue("@forumId", forumId);
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
                if (conn != null) conn.Close();
            }
        }

        public static Forum GetForumById(int forumId)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT forumId, courseId, title, postFlair FROM Forum WHERE forumId = @forumId", conn))
                {
                    cmd.Parameters.AddWithValue("@forumId", forumId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int fId = UtilityDAL.returnInt(reader, "forumId");
                            int cId = UtilityDAL.returnInt(reader, "courseId");
                            string title = UtilityDAL.returnString(reader, "title");
                            string postFlair = UtilityDAL.returnString(reader, "postFlair");
                            return new Forum(fId, cId, title, postFlair);
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
                if (conn != null) conn.Close();
            }
            return null;
        }
    }
}