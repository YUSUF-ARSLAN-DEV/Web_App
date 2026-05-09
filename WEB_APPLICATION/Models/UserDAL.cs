using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient ;
using BCrypt.Net; 
using System.Data ; 
using System.Text.RegularExpressions;

namespace WEB_APPLICATION.Models
{
    public class UserDAL
    {
        public static bool CheckValidCredentials(string userName , string password) 
        {
            if (string.IsNullOrEmpty(userName)) {return false ; }
            if (userName.Length <4  || userName.Length > 20 ) {return false ; }
            if (!Regex.IsMatch(userName , @"^[a-zA-Z0-9_]+$"))  return false ; 
            if (char.IsDigit(userName[0])) return false ; 
            if (string.IsNullOrEmpty(password)) return false ; 
            if (!Regex.IsMatch(password,@"[A-Z]")) return false ;
            if (!Regex.IsMatch(password,@"[a-z]")) return false ;
            if (!Regex.IsMatch(password,@"[0-9]")) return false ;
            return true ; 
        }

        public static bool RegisterUser(string username , string password , User.Role userRole , string firstName , string lastName)  
        {
            SqlConnection conn = null;
            try 
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                conn = UtilityDAL.createConnection();
                using ( SqlCommand insert = new SqlCommand("INSERT INTO [User] (userName , [password],role , firstName, lastName , accountCreationDate ) VALUES (@userName , @password, @role , @firstName , @lastName , @accountCreationDate ) ", conn) ) 
                {
                    insert.Parameters.AddWithValue("@userName" , username ) ;
                    insert.Parameters.AddWithValue("@password",  hashedPassword ) ;
                    insert.Parameters.AddWithValue("@role", UtilityDAL.roleToString(userRole)) ;
                    insert.Parameters.AddWithValue("@firstName", firstName ) ;
                    insert.Parameters.AddWithValue("@lastName",  lastName ) ;
                    insert.Parameters.AddWithValue("@accountCreationDate", DateTime.Now) ;
                    conn.Open();
                    insert.ExecuteNonQuery()  ; 
                } 
                return true ; 
            } 
            catch (SqlException e ) {return false ; } 
            finally {if (conn != null) conn.Close() ;}
        }

        public static int LoginAuthentication(string userName, string password)
        {
            string passwordReturned  ;
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using( SqlCommand cmd = new SqlCommand("SELECT [password] FROM [User] WHERE userName = @userName", conn)) 
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) 
                    {
                        if (!reader.Read()) { return 2; }
                        passwordReturned = reader["password"].ToString();
                    }
                }
                bool correctPassword = BCrypt.Net.BCrypt.Verify(password, passwordReturned);
                if (!correctPassword) { return 1; }
                return 0;
            }
            catch (SqlException e)
            {
                return -1;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        public static User GetUserById(int userId)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE userId = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == false)
                        {
                            return null;
                        }
                        int id = UtilityDAL.returnInt(reader, "userId");
                        string userName = UtilityDAL.returnString(reader, "userName");
                        string password = UtilityDAL.returnString(reader, "password");
                        User.Role role = UtilityDAL.parseStringToRole(UtilityDAL.returnString(reader, "role"));
                        string firstName = UtilityDAL.returnString(reader, "firstName");
                        string lastName = UtilityDAL.returnString(reader, "lastName");
                        DateTime datetime = UtilityDAL.returnDateTime(reader, "accountCreationDate");
                        User user = new User(id, userName, password, role, firstName, lastName, datetime);
                        return user;
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
        }

        public static User GetUserByUsername(string userName)
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE userName = @userName", conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;
                        int id = UtilityDAL.returnInt(reader, "userId");
                        string uName = UtilityDAL.returnString(reader, "userName");
                        string password = UtilityDAL.returnString(reader, "password");
                        User.Role role = UtilityDAL.parseStringToRole(UtilityDAL.returnString(reader, "role"));
                        string firstName = UtilityDAL.returnString(reader, "firstName");
                        string lastName = UtilityDAL.returnString(reader, "lastName");
                        DateTime datetime = UtilityDAL.returnDateTime(reader, "accountCreationDate");
                        return new User(id, uName, password, role, firstName, lastName, datetime);
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
        }

        public static List<User> GetAllUsers(string userRole)
        {
            if (userRole == null ) {userRole = "student" ;}
            List<User> specifiedUserList = new List<User>();
            SqlConnection conn = null;
            try 
            {
                conn = UtilityDAL.createConnection();
                using ( SqlCommand cmd = new SqlCommand("SELECT * FROM [User] WHERE role = @wantedRole", conn ))  
                {
                    cmd.Parameters.AddWithValue("@wantedRole", userRole  ) ;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader() ) 
                    {
                        while (reader.Read() ) 
                        {
                            int id = UtilityDAL.returnInt(reader, "userId" ) ; 
                            string userName = UtilityDAL.returnString(reader,"userName") ;
                            string  password = UtilityDAL.returnString(reader,"password") ; 
                            User.Role readRole  = UtilityDAL.parseStringToRole(UtilityDAL.returnString(reader,"role")) ;
                            string firstName = UtilityDAL.returnString(reader,"firstName") ;
                            string lastName =  UtilityDAL.returnString(reader,"lastName") ;
                            DateTime datetime = UtilityDAL.returnDateTime(reader,"accountCreationDate");
                            User user = new User(id , userName , password , readRole , firstName , lastName , datetime );
                            specifiedUserList.Add(user) ;
                        }
                    }
                }
                return specifiedUserList ; 
            }
            catch (SqlException e ) 
            {
                return null ; 
            }
            finally 
            {
                if (conn != null) conn.Close();
            }
        }

        public static bool UpdateUserProfile(int userId, string firstName = "", string lastName = "")
        {
            if (firstName == "" && lastName == "") { return false; }
            
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                string query;
                SqlCommand cmd;

                if (lastName == "")
                    query = "UPDATE [User] SET firstName = @firstName WHERE userId = @userId";
                else if (firstName == "")
                    query = "UPDATE [User] SET lastName = @lastName WHERE userId = @userId";
                else
                    query = "UPDATE [User] SET firstName = @firstName, lastName = @lastName WHERE userId = @userId";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                if (firstName != "") cmd.Parameters.AddWithValue("@firstName", firstName);
                if (lastName != "") cmd.Parameters.AddWithValue("@lastName", lastName);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (SqlException e) { return false; }
            finally { if (conn != null) conn.Close(); }
        }

        public static bool UpdatePassword(int userId, string newPassword)
        {
            SqlConnection conn = null;
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                conn = UtilityDAL.createConnection();
                SqlCommand cmd = new SqlCommand("UPDATE [User] SET [password] = @hashedPassword WHERE userId = @userId", conn);
                cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (SqlException e) { return false; }
            finally { if (conn != null) conn.Close(); }
        }

        public static bool DeleteUser(int userId) 
        {
            SqlConnection conn = null;
            try
            {
                conn = UtilityDAL.createConnection();
                SqlCommand cmd = new SqlCommand("DELETE FROM [User] WHERE userId = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (SqlException e) { return false; }
            finally { if (conn != null) conn.Close(); }
        }
    }
}