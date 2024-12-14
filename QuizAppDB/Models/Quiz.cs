using System.Collections.Generic;

namespace QuizAppDB.Models
{
    public class Quiz
    {
        public int Id { get; set; } // Primary key for the quiz
        public string Title { get; set; } = string.Empty; // Title of the quiz
        public bool ShowCorrectAnswers { get; set; } // Whether to show correct answers after incorrect attempts
        public List<Question> Questions { get; set; } = new(); // List of questions in the quiz
    }
}