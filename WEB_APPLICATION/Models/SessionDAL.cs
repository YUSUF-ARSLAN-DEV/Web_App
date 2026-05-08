using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sqlclient ; 

namespace WEB_APPLICATION.Models
{
    public class SessionDAL
    {
        private SqlConnection conn =  UtilityDAl.createConnection() ; 


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
    }
}