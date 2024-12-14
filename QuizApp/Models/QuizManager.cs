using QuizApp.Models; // Importing the Quiz and Question models
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace QuizApp.Persistence
{
    // Handles loading, saving, and managing quizzes
    public class QuizManager
    {
        // Path to the file where quizzes are stored
        private readonly string _filePath;

        // List of quizzes managed by this instance
        public List<Quiz> Quizzes { get; set; } = new List<Quiz>();

        // Constructor accepting a file path for quiz storage
        public QuizManager(string filePath)
        {
            _filePath = filePath;
        }

        // Adds a new quiz to the manager
        public void AddQuiz(Quiz quiz)
        {
            Quizzes.Add(quiz);
        }

        // Saves the current list of quizzes to a JSON file
        public void SaveQuizzesToFile()
        {
            var json = JsonSerializer.Serialize(Quizzes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        // Loads quizzes from a JSON file into the manager
        public void LoadQuizzesFromFile()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                Quizzes = JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
            }
        }

        // Finds a quiz by title
        public Quiz? FindQuizByTitle(string title)
        {
            return Quizzes.Find(q => q.Title.Equals(title, System.StringComparison.OrdinalIgnoreCase));
        }

        // Removes a quiz by title
        public bool RemoveQuizByTitle(string title)
        {
            var quiz = FindQuizByTitle(title);
            if (quiz != null)
            {
                Quizzes.Remove(quiz);
                return true;
            }
            return false;
        }

        // Returns the total number of quizzes
        public int GetTotalQuizzes()
        {
            return Quizzes.Count;
        }
    }
}
