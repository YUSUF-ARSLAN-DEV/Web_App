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
        public bool  logLogin(int userId )
        {
            bool success = false;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO [Session] (userId, loginDate, loginTime , logoutTime ) VALUES (@ , @ , @   , @    )", conn);
                cmd.Parameters.AddWithValue("@val1", value1);
                cmd.Parameters.AddWithValue("@val2", value2);
                if (cmd.ExecuteNonQuery() > 0)
                    success = true;
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;
        }
    }
}