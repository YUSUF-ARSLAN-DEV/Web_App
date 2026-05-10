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
        private SqlConnection conn = UtilityDAL.createConnection();

        public bool CheckValidCredentials(string userName , string password) 
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

        public int RegisterUser(string username, string password, User.Role userRole, string firstName, string lastName)
        {
            try { 
               
                if (!CheckValidCredentials(username, password)) return 1; // invalid credentials
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                using (SqlCommand insert = new SqlCommand("INSERT INTO [User] (userName, [password], role, firstName, lastName, accountCreationDate) VALUES (@userName, @password, @role, @firstName, @lastName, @accountCreationDate)", conn))
                {
                    insert.Parameters.AddWithValue("@userName", username);
                    insert.Parameters.AddWithValue("@password", hashedPassword);
                    insert.Parameters.AddWithValue("@role", UtilityDAL.roleToString(userRole));
                    insert.Parameters.AddWithValue("@firstName", firstName);
                    insert.Parameters.AddWithValue("@lastName", lastName);
                    insert.Parameters.AddWithValue("@accountCreationDate", DateTime.Now);
                    conn.Open();
                    insert.ExecuteNonQuery();
                }
                return 0; // success
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return e.Number; // returns SQL error number
            }
            finally {  conn.Close(); }
        }

        public int LoginAuthentication(string userName, string password)
        {
            string passwordReturned  ;
            try
            {
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
                 conn.Close();
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
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
                 conn.Close();
            }
        }

        public User GetUserByUsername(string userName)
        {
            try
            {
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
                 conn.Close();
            }
        }

        public List<User> GetAllUsers(string userRole)
        {
            if (userRole == null ) {userRole = "student" ;}
            List<User> specifiedUserList = new List<User>();
            try 
            {
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
                 conn.Close();
            }
        }

        public bool UpdateUserProfile(int userId, string firstName = "", string lastName = "")
        {
            if (firstName == "" && lastName == "") { return false; }
            
            try
            {
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
            finally {  conn.Close(); }
        }

        public bool UpdatePassword(int userId, string newPassword)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                SqlCommand cmd = new SqlCommand("UPDATE [User] SET [password] = @hashedPassword WHERE userId = @userId", conn);
                cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (SqlException e) { return false; }
            finally {  conn.Close(); }
        }

        public bool DeleteUser(int userId) 
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM [User] WHERE userId = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (SqlException e) { return false; }
            finally {  conn.Close(); }
        }
    }
}