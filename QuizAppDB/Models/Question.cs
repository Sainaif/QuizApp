using System.Collections.Generic;

namespace QuizAppDB.Models
{
    public class Question
    {
        public int Id { get; set; } // Primary key for the question
        public string Text { get; set; } = string.Empty; // Text of the question
        public int QuizId { get; set; } // Foreign key referencing the quiz
        public Quiz Quiz { get; set; } = null!; // Navigation property to the quiz
        public List<Choice> Choices { get; set; } = new(); // List of choices for the question
    }
}