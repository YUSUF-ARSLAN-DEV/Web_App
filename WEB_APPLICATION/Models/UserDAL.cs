using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient ;
using BCrypt.Net; 
using System.Data ; 
using System.Text.RegularExpressions; // this allows us to validate password 


namespace WEB_APPLICATION.Models
{
    public class UserDAL // DAL basically stands for Data Access Layer 
    {   // THE METHOD BELOW RETURNS FALSE IF THE USERNAME ALREADY EXISTS 
       private SqlConnection conn = UtilityDAL.createConnection();
       public bool   checkValidCredentials(string userName , string password ) 
       {

        if (string.IsNullOrEmpty(userName)) {return false ; }// check for empty or null passwords 
        if (userName.Length <4  || userName.Length > 20 ) {return false ; }
        // a username can only be letters numbers or underscores no special character
        if (!Regex.IsMatch(userName , @"^[a-zA-Z0-9_]+$"))  return false ; 
        if (char.IsDigit(userName[0])) return false ; // user name can not start with digit 
        if (string.IsNullOrEmpty(password)) return false ; // pass can not be null or empty 
        if (!Regex.IsMatch(password,@"[A-Z]")) return false ; // makes sure it has at least one upperCase
        if (!Regex.IsMatch(password,@"[a-z]")) return false ; // makes sure it has at least one lower case 
        if (!Regex.IsMatch(password,@"[0-9]")) return false ; // the password must at least has one digit 
        // if non of these run then return true 
        return true ; 
       }
        public bool  registerUser(string username , string password , User.Role userRole , string firstName , string lastName )  
        {
           
             try {
                
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password) ;
                
                conn.Open() ; 
                // we do not check first for the user name cause we alreadyt set the column to be UNIQUE In the data base 
               using ( SqlCommand insert = new SqlCommand("INSERT INTO [User] (userName , [password],role , firstName, lastName , accountCreationDate ) VALUES (@userName , @password, @role , @firstName , @lastName , @accountCreationDate ) ", conn) ) {
                insert.Parameters.AddWithValue("@userName" , username ) ;
                insert.Parameters.AddWithValue("@password",  hashedPassword ) ;
                insert.Parameters.AddWithValue("@role", UtilityDAL.roleToString(userRole)) ;
                insert.Parameters.AddWithValue("@firstName", firstName ) ;
                insert.Parameters.AddWithValue("@lastName",  lastName ) ;
                insert.Parameters.AddWithValue("@accountCreationDate", DateTime.Now) ;
                
                insert.ExecuteNonQuery()  ; 
                } 
                return true ; 
             } catch (SqlException e ) {return false ; } 
             finally {conn.Close() ;}
        }
        public  int LoginAuthentication(string userName, string password)
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
                        if (!reader.Read()) { return 2; } // username not found
                        passwordReturned = reader["password"].ToString();
                    
                    }
                
                }
                bool correctPassword = BCrypt.Net.BCrypt.Verify(password, passwordReturned);

                if (!correctPassword) { return 1; } // wrong password
                return 0; // success
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
        public User getUserById(int userId) // retrives the user info by the User's ID 
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
        public  List<User> getAllUsers(string userRole)  // this is an Admin only method 
        {
            if (userRole == null ) {userRole = "student" ;} // defualt role is student   
            List<User> specifiedUserList = new List<User>(); 
            try 
            {
                conn.Open()  ; 
                
                using ( SqlCommand cmd = new SqlCommand("SELECT * FROM [User] WHERE role = @wantedRole", conn ))  
                {
                    cmd.Parameters.AddWithValue("@wantedRole", userRole  ) ;
                    // connection must be open before execution 
                    using (SqlDataReader reader = cmd.ExecuteReader() ) 
                    {
                        while (reader.Read() ) // an automatic check for True and False is like done  
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
                    } // closing the reader after the loop finished 
                }
                return specifiedUserList ; 
            }
            catch (SqlException e ) 
                {
                    return null ; 
                }
            finally 
            {
                conn.Close() ;
            }
        }  
        public bool  updateUserProfile(int userId, string firstName = "", string lastName = "")
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
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException e) { return false; }
            finally { conn.Close(); }
        }
        public  bool UpdatePassword(int userId, string newPassword)
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
            finally { if (conn != null) conn.Close(); }
        }
        public  bool deleteUser(int userId ) 
        {
            try
            {

                SqlCommand cmd = new SqlCommand("DELETE  FROM [User] WHERE   userId = @userId", conn);
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