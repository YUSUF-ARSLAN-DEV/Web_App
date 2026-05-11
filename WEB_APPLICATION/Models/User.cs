using System;
using BCrypt.Net; 
namespace WEB_APPLICATION.Models
{



    public class User
    {
        public bool activeStatus { get; set; }
        public  int userId { get; set; }
        public  string userName  { get; set; }
        public  string password { get; set; }
        public  Role role { get; set; }
        public  string firstName {get ; set; }
        public  string lastName {get; set; }
        public DateTime accountCreationDate {get; set ; } // interesting short cut 
        public enum Role
        {
            Admin,
            Instructor,
            Student
        }
// Constructor for reading from DB (userId already exists)
    public User(int userId, string userName, string password, Role role, string firstName, string lastName, DateTime accountCreationDate) 
    {
        this.userId = userId ;  
        this.userName = userName ; 
        this.password = password ; 
        this.role = role ; 
        this.firstName = firstName ; 
        this.lastName = lastName ; 
        this.accountCreationDate = accountCreationDate ; 
    }



        // Constructor for reading from DB (add activeStatus at the end)
        public User(int userId, string userName, string password, Role role, string firstName, string lastName, DateTime accountCreationDate, bool activeStatus)
        {
            this.userId = userId;
            this.userName = userName;
            this.password = password;
            this.role = role;
            this.firstName = firstName;
            this.lastName = lastName;
            this.accountCreationDate = accountCreationDate;
            this.activeStatus = activeStatus;
        }

        // Constructor for creating new user (activeStatus defaults to true)
        public User(string userName, string password, Role role, string firstName, string lastName)
        {
            this.userId = 0;
            this.userName = userName;
            this.password = password;
            this.role = role;
            this.firstName = firstName;
            this.lastName = lastName;
            this.accountCreationDate = DateTime.Now;
            this.activeStatus = true;
        }

        // Add property
       

    }
}