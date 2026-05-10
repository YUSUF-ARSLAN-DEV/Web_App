using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System;

namespace WEB_APPLICATION.Models
{
    public class Rating
    {
        public int ratingId { get; set; }
        public int courseId { get; set; }
        public int userId { get; set; }
        public int score { get; set; }
        public string comment { get; set; }
        public DateTime ratingDate { get; set; }

        // constructor for creating a new rating
        public Rating(int courseId, int userId, int score, string comment)
        {
            this.courseId = courseId;
            this.userId = userId;
            this.score = score;
            this.comment = comment;
            this.ratingDate = DateTime.Now;
        }

        // constructor for reading from DB
        public Rating(int ratingId, int courseId, int userId, int score, string comment, DateTime ratingDate)
        {
            this.ratingId = ratingId;
            this.courseId = courseId;
            this.userId = userId;
            this.score = score;
            this.comment = comment;
            this.ratingDate = ratingDate;
        }
    }
}