using System;


namespace WEB_APPLICATION.Models
{
    public class Session
    {
        public int SessionID { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }

        public Session(int sessionID, int userID) // this is a constructor to register a new login 
        {
            SessionID = sessionID;
            UserID = userID;
            Date = DateTime.Now; // this returns the data nad itme but what gets stored in data base is only date 
            LoginTime = DateTime.UtcNow;
        }
    }
}
