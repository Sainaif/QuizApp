namespace QuizAppDB.Models
{
    public class Choice
    {
        public int Id { get; set; } // Primary key for the choice
        public string Text { get; set; } = string.Empty; // Text of the choice
        public bool IsCorrect { get; set; } // Whether the choice is correct
        public int QuestionId { get; set; } // Foreign key referencing the question
        public Question Question { get; set; } = null!; // Navigation property to the question
    }
}