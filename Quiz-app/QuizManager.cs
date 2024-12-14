// Class managing quiz operations
using System.Text.Json;

public class QuizManager
{
    public List<Quiz> Quizzes { get; set; } = new List<Quiz>(); // List of all quizzes

    // Add a quiz to the manager
    public void AddQuiz(Quiz quiz) => Quizzes.Add(quiz);

    // Save quizzes to a JSON file
    public void SaveQuizzesToFile(string filePath)
    {
        var json = JsonSerializer.Serialize(Quizzes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    // Load quizzes from a JSON file
    public void LoadQuizzesFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            Quizzes = JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
        }
    }
}