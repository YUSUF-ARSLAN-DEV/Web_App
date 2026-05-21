using System;
using BCrypt.Net;

namespace WEB_APPLICATION.Models
{
    public class User
    {
        public bool activeStatus { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public Role role { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime accountCreationDate { get; set; }

        // Auth extensions
        public string email { get; set; }
        public string googleId { get; set; }
        public bool emailVerified { get; set; }

        public enum Role
        {
            Admin,
            Instructor,
            Student
        }

        // Full constructor — reading from DB with all columns
        public User(int userId, string userName, string password, Role role,
                    string firstName, string lastName, DateTime accountCreationDate,
                    bool activeStatus, string email = null, string googleId = null,
                    bool emailVerified = false)
        {
            this.userId = userId;
            this.userName = userName;
            this.password = password;
            this.role = role;
            this.firstName = firstName;
            this.lastName = lastName;
            this.accountCreationDate = accountCreationDate;
            this.activeStatus = activeStatus;
            this.email = email;
            this.googleId = googleId;
            this.emailVerified = emailVerified;
        }

        // Legacy constructor (without activeStatus — backward compat)
        public User(int userId, string userName, string password, Role role,
                    string firstName, string lastName, DateTime accountCreationDate)
        {
            this.userId = userId;
            this.userName = userName;
            this.password = password;
            this.role = role;
            this.firstName = firstName;
            this.lastName = lastName;
            this.accountCreationDate = accountCreationDate;
            this.activeStatus = true;
        }

        // Constructor for creating a new regular user (no userId yet)
        public User(string userName, string password, Role role,
                    string firstName, string lastName, string email = null)
        {
            this.userId = 0;
            this.userName = userName;
            this.password = password;
            this.role = role;
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.accountCreationDate = DateTime.Now;
            this.activeStatus = true;
            this.emailVerified = false;
        }
    }
}
