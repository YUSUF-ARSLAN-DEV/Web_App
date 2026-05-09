using System;

namespace WEB_APPLICATION.Models
{
public class Assessment
{
    public int assessmentId { get; set; }
    public int lessonId { get; set; }
    public int attemptNumber { get; set; }

    public Assessment(int assessmentId, int lessonId)
    {
        this.assessmentId = assessmentId;
        this.lessonId = lessonId;
        this.attemptNumber = 0;
    }
}
}
