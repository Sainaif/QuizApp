// Class representing a question in the quiz
public class Question
{
    public string Text { get; set; } = string.Empty; // The text of the question
    public List<string> Choices { get; set; } = new List<string>(); // List of answer choices
    public List<int> CorrectChoices { get; set; } = new List<int>(); // List of indices of correct answers
}