public class Question
{
    public string Text { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = new List<string>();
    public List<int> CorrectChoices { get; set; } = new List<int>();
}