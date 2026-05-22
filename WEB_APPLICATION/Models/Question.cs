using System;

namespace WEB_APPLICATION.Models
{
public class Question
{
    public int questionId { get; set; }
    public int assessmentId { get; set; }
    public string questionType { get; set; }
    public string questionText { get; set; }
    public string questionAnswer { get; set; }
    public string correctAnswer { get; set; }

    public Question(int questionId, int assessmentId, string questionType, string questionText, string correctAnswer)
    {
        this.questionId = questionId;
        this.assessmentId = assessmentId;
        this.questionType = questionType;
        this.questionText = questionText;
        this.correctAnswer = correctAnswer;
        this.questionAnswer = null;
    }
        // a constructor without questionId for creating new questions before they are saved to the database 
        public Question( int assessmentId, string questionType, string questionText, string correctAnswer)
        {
            this.assessmentId = assessmentId;
            this.questionType = questionType;
            this.questionText = questionText;
            this.correctAnswer = correctAnswer;
            this.questionAnswer = null;
        }
    }
}
