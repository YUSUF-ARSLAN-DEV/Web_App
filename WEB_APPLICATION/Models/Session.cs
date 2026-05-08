using System;


namespace WEB_APPLICATION.Models
{
using System;

namespace WEB_APPLICATION.Models
{
    public class Session
    {
        public int sessionId { get; set; }
        public int userId { get; set; }
        public DateTime date { get; set; }
        public TimeSpan loginTime { get; set; }
        public TimeSpan logoutTime { get; set; }

        public Session(int userId) // this constructor creates a new login session
        {
            this.userId = userId;
            this.date = DateTime.Now.Date;
            this.loginTime = DateTime.Now.TimeOfDay;
        }
        public Session(int sessionId, int userId, DateTime date, TimeSpan loginTime, TimeSpan logoutTime)
        {
            this.sessionId = sessionId;
            this.userId = userId;
            this.date = date;
            this.loginTime = loginTime;
            this.logoutTime = logoutTime;
        }
    }
}
}
