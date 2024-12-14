// Class representing a quiz
public class Quiz
{
    public string Title { get; set; } = string.Empty; // The title of the quiz
    public bool ShowCorrectAnswers { get; set; } // Whether to show correct answers after wrong attempts
    public List<Question> Questions { get; set; } = new List<Question>(); // List of questions in the quiz
}
