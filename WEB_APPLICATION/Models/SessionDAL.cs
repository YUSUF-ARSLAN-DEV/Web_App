using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient ; 

namespace WEB_APPLICATION.Models
{
    public class SessionDAL
    {
        private SqlConnection conn =  UtilityDAL.createConnection() ; 


        // the method below registers when a user logs in in the system - CRUD - insertion 
        public int  logLogin(int userId ) // this method should return the session id of the user if it returns -1 then that means an error occured 
        {
            Session session = new Session(userId) ; 
            int id = -1 ; 
            try
            {
                conn.Open();
               using (  SqlCommand cmd = new SqlCommand("INSERT INTO [Session] (userId, loginDate, loginTime  ) VALUES (@userId , @loginDate , @loginTime); SELECT  SCOPE_IDENTITY() ", conn)) {
                        cmd.Parameters.AddWithValue("@userId", session.userId);
                        cmd.Parameters.AddWithValue("@loginDate", session.date);
                        cmd.Parameters.AddWithValue("@loginTime", session.loginTime);
                 Object sessionId = cmd.ExecuteScalar() ; 
                    if (sessionId != null ) // meaining a value was returned ; 
                    {
                        id  = Convert.ToInt32(sessionId) ; 
                    }
                }
               
            }    
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return id;
        }
    
        public bool logLogout(int sessionId )
        {
            TimeSpan logoutTime = DateTime.Now.TimeOfDay ; 
            bool success = false;
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE [Session] SET logoutTime = @logoutTime WHERE sessionId = @sessionId", conn))
                {
                    cmd.Parameters.AddWithValue("@logoutTime", logoutTime);
                    cmd.Parameters.AddWithValue("@sessionId", sessionId);
                    if (cmd.ExecuteNonQuery() > 0)
                        success = true;
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;

        }
    
        public List<Session> getSessionsByUser(int userId ) // this is an Admin Method that shows all of the Sessions for a specific user 
        {
            List<Session> userSessions = new List<Session>() ; 
            try // returns 
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand( 
                    "SELECT * FROM [Session] WHERE userId = @userId ORDER BY loginDate DESC ", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userSessions.Add(new Session(
                                UtilityDAL.returnInt(reader, "sessionId"),
                                UtilityDAL.returnInt(reader, "userId"),
                                UtilityDAL.returnDateTime(reader, "loginDate"),
                                UtilityDAL.returnTimeSpan(reader, "loginTime"),
                                UtilityDAL.returnTimeSpan(reader, "logoutTime")
                            ));
                        }
                    }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return userSessions ; 
        } 
        
        
    
    }
}