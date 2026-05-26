using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace WEB_APPLICATION.Models
{
    public class UserDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection();

        // -------------------------------------------------------
        // Helpers - basically this makes our life a crap ton easier by centralizing all the logic for validating credentials and mapping DB rows to User objects, so we don't have to repeat it in every method
        // -------------------------------------------------------

        public bool CheckValidCredentials(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName)) return false;
            if (userName.Length < 4 || userName.Length > 20) return false;
            if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9_]+$")) return false;
            if (char.IsDigit(userName[0])) return false;
            if (string.IsNullOrEmpty(password)) return false;
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;
            if (!Regex.IsMatch(password, @"[a-z]")) return false;
            if (!Regex.IsMatch(password, @"[0-9]")) return false;
            return true;
        }

        // Reads all columns including the new auth columns from an open SqlDataReader row
        private User MapUser(SqlDataReader reader)
        {
            int id = UtilityDAL.returnInt(reader, "userId");
            string userName = UtilityDAL.returnString(reader, "userName");
            string password = UtilityDAL.returnString(reader, "password");
            User.Role role = UtilityDAL.parseStringToRole(UtilityDAL.returnString(reader, "role"));
            string firstName = UtilityDAL.returnString(reader, "firstName");
            string lastName = UtilityDAL.returnString(reader, "lastName");
            DateTime datetime = UtilityDAL.returnDateTime(reader, "accountCreationDate");
            bool activeStatus = UtilityDAL.returnBit(reader, "activeStatus");
            string email = UtilityDAL.returnString(reader, "email");
            string googleId = UtilityDAL.returnString(reader, "googleId");
            bool emailVerified = UtilityDAL.returnBit(reader, "emailVerified");
            return new User(id, userName, password, role, firstName, lastName,
                            datetime, activeStatus, email, googleId, emailVerified);
        }

        // -------------------------------------------------------
        // Registration
        // -------------------------------------------------------


        public int RegisterUser(string username, string password, User.Role userRole,
                                string firstName, string lastName, string email)
        {
            try
            {
                if (!CheckValidCredentials(username, password)) return 1;
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO [User] (userName, [password], role, firstName, lastName, " +
                    "accountCreationDate, activeStatus, email, emailVerified) " +
                    "VALUES (@userName, @password, @role, @firstName, @lastName, " +
                    "@accountCreationDate, @activeStatus, @email, @emailVerified)", conn))
                {
                    cmd.Parameters.AddWithValue("@userName", username);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@role", UtilityDAL.roleToString(userRole));
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@accountCreationDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@activeStatus", true);
                    cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@emailVerified", true); // code was verified before calling this
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return 0;
            }
            catch (SqlException e)
            {
                return e.Number;
            }
            finally { conn.Close(); }
        }

        /// <summary>
        /// Registers a Google-authenticated user.
        /// Returns the new userId on success, -1 on failure.
        /// </summary>
        public int RegisterGoogleUser(string googleId, string email, string firstName,
                                      string lastName, User.Role userRole, string autoUsername)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO [User] (userName, [password], role, firstName, lastName, " +
                    "accountCreationDate, activeStatus, email, googleId, emailVerified) " +
                    "OUTPUT INSERTED.userId " +
                    "VALUES (@userName, NULL, @role, @firstName, @lastName, " +
                    "@accountCreationDate, @activeStatus, @email, @googleId, @emailVerified)", conn))
                {
                    cmd.Parameters.AddWithValue("@userName", autoUsername);
                    cmd.Parameters.AddWithValue("@role", UtilityDAL.roleToString(userRole));
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@accountCreationDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@activeStatus", true);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@googleId", googleId);
                    cmd.Parameters.AddWithValue("@emailVerified", true); // Google email is pre-verified
                    conn.Open();
                    int newId = (int)cmd.ExecuteScalar();
                    return newId;
                }
            }
            catch (SqlException)
            {
                return -1;
            }
            finally { conn.Close(); }
        }

        // -------------------------------------------------------
        // Login
        // -------------------------------------------------------

        /// <summary>
        /// Returns 0=success, 1=wrong password, 2=not found, 3=deactivated.
        /// </summary>
        public int LoginAuthentication(string userName, string password)
        {
            string passwordReturned;
            bool activeStatus;
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT [password], activeStatus FROM [User] WHERE userName = @userName", conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return 2;
                        passwordReturned = reader["password"].ToString();
                        activeStatus = UtilityDAL.returnBit(reader, "activeStatus");
                    }
                }
                if (!activeStatus) return 3;
                // Google-only users have no password
                if (string.IsNullOrEmpty(passwordReturned)) return 4;
                bool correct = BCrypt.Net.BCrypt.Verify(password, passwordReturned);
                return correct ? 0 : 1;
            }
            catch (SqlException) { return -1; }
            finally { conn.Close(); }
        }

        // -------------------------------------------------------
        // Lookups
        // -------------------------------------------------------

        public User GetUserById(int userId)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE userId = @userId AND activeStatus = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return MapUser(reader);
                    }
                }
            }
            catch (SqlException) { return null; }
            finally { conn.Close(); }
        }

        public User GetUserByUsername(string userName)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE userName = @userName AND activeStatus = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return MapUser(reader);
                    }
                }
            }
            catch (SqlException) { return null; }
            finally { conn.Close(); }
        }

        public User FindByGoogleId(string googleId)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE googleId = @googleId AND activeStatus = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@googleId", googleId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return MapUser(reader);
                    }
                }
            }
            catch (SqlException) { return null; }
            finally { conn.Close(); }
        }

        public User FindByEmail(string email)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE email = @email AND activeStatus = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return MapUser(reader);
                    }
                }
            }
            catch (SqlException) { return null; }
            finally { conn.Close(); }
        }

        /// <summary>
        /// Checks whether a given username already exists in the DB.
        /// </summary>
        public bool UsernameExists(string userName)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM [User] WHERE userName = @userName", conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    conn.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (SqlException) { return false; }
            finally { conn.Close(); }
        }

        // -------------------------------------------------------
        // Bulk reads (unchanged logic, updated to use MapUser)
        // -------------------------------------------------------

        public List<User> GetUsersByRole(string userRole)
        {
            var list = new List<User>();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE role = @role AND activeStatus = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@role", userRole);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                        while (reader.Read()) list.Add(MapUser(reader));
                }
                return list;
            }
            catch (SqlException) { return null; }
            finally { conn.Close(); }
        }

        public List<User> GetAllActiveNonAdminUsers()
        {
            var list = new List<User>();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE role != 'admin' AND activeStatus = 1", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                        while (reader.Read()) list.Add(MapUser(reader));
                }
                return list;
            }
            catch (SqlException) { return null; }
            finally { conn.Close(); }
        }

        // -------------------------------------------------------
        // Updates (unchanged)
        // -------------------------------------------------------

        public bool UpdateUserProfile(int userId, string firstName = "", string lastName = "")
        {
            if (firstName == "" && lastName == "") return false;
            try
            {
                string query;
                if (lastName == "")
                    query = "UPDATE [User] SET firstName = @firstName WHERE userId = @userId AND activeStatus = 1";
                else if (firstName == "")
                    query = "UPDATE [User] SET lastName = @lastName WHERE userId = @userId AND activeStatus = 1";
                else
                    query = "UPDATE [User] SET firstName = @firstName, lastName = @lastName WHERE userId = @userId AND activeStatus = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                if (firstName != "") cmd.Parameters.AddWithValue("@firstName", firstName);
                if (lastName != "") cmd.Parameters.AddWithValue("@lastName", lastName);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (SqlException) { return false; }
            finally { conn.Close(); }
        }

        public bool UpdatePassword(int userId, string newPassword)
        {
            try
            {
                string hashed = BCrypt.Net.BCrypt.HashPassword(newPassword);
                SqlCommand cmd = new SqlCommand(
                    "UPDATE [User] SET [password] = @hashed WHERE userId = @userId AND activeStatus = 1", conn);
                cmd.Parameters.AddWithValue("@hashed", hashed);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (SqlException) { return false; }
            finally { conn.Close(); }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE [User] SET activeStatus = 0 WHERE userId = @userId AND activeStatus = 1", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (SqlException) { return false; }
            finally { conn.Close(); }
        }
    }
}
