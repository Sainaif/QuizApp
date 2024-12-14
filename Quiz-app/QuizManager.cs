using System.Text.Json;

public class QuizManager
{
    public List<Quiz> Quizzes { get; set; } = new List<Quiz>();

    public void AddQuiz(Quiz quiz) => Quizzes.Add(quiz);

    public void SaveQuizzesToFile(string filePath)
    {
        var json = JsonSerializer.Serialize(Quizzes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public void LoadQuizzesFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            Quizzes = JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
        }
    }
}