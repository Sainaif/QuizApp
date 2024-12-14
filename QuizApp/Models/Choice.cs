namespace QuizApp.Models
{
    // Represents a single answer choice for a question
    public class Choice
    {
        // The text of the answer choice
        public string Text { get; set; } = string.Empty;

        // Indicates whether this choice is correct
        public bool IsCorrect { get; set; } = false;

        // Optional constructor for easier initialization
        public Choice(string text, bool isCorrect)
        {
            Text = text;
            IsCorrect = isCorrect;
        }

        // Parameterless constructor for serialization/deserialization
        public Choice() { }
    }
}
