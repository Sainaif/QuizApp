using System.Collections.Generic;

namespace QuizApp.Models
{
    // Represents a question in a quiz
    public class Question
    {
        // The text of the question
        public string Text { get; set; } = string.Empty;

        // List of answer choices for this question
        public List<Choice> Choices { get; set; } = new List<Choice>();

        // Adds a choice to the question
        public void AddChoice(string text, bool isCorrect)
        {
            Choices.Add(new Choice(text, isCorrect));
        }

        // Parameterless constructor for serialization/deserialization
        public Question() { }

        // Constructor for easier initialization
        public Question(string text, List<Choice> choices)
        {
            Text = text;
            Choices = choices;
        }

        // Returns the correct answer(s) for this question
        public List<Choice> GetCorrectChoices()
        {
            return Choices.FindAll(choice => choice.IsCorrect);
        }

        // Returns the incorrect answer(s) for this question
        public List<Choice> GetIncorrectChoices()
        {
            return Choices.FindAll(choice => !choice.IsCorrect);
        }
    }
}
