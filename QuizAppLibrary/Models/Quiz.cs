using System.Collections.Generic;

namespace QuizApp.Models
{
    // Represents a quiz
    public class Quiz
    {
        // The title of the quiz
        public string Title { get; set; } = string.Empty;

        // Determines whether correct answers are shown after incorrect attempts
        public bool ShowCorrectAnswers { get; set; } = false;

        // List of questions in the quiz
        public List<Question> Questions { get; set; } = new List<Question>();

        // Adds a question to the quiz
        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }

        // Parameterless constructor for serialization/deserialization
        public Quiz() { }

        // Constructor for easier initialization
        public Quiz(string title, bool showCorrectAnswers)
        {
            Title = title;
            ShowCorrectAnswers = showCorrectAnswers;
        }

        // Returns the total number of questions in the quiz
        public int GetTotalQuestions()
        {
            return Questions.Count;
        }

        // Returns all questions with at least one correct answer
        public List<Question> GetQuestionsWithCorrectAnswers()
        {
            return Questions.FindAll(q => q.GetCorrectChoices().Count > 0);
        }
    }
}
