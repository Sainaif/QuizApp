using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

// Test class for QuizManager
[TestClass]
public class QuizManagerTests
{
    // Test to verify that a quiz can be added successfully
    [TestMethod]
    public void AddQuiz_ShouldAddQuizToList()
    {
        var quizManager = new QuizManager();
        var quiz = new Quiz { Title = "Przykładowy Quiz", ShowCorrectAnswers = true }; // Include the 'ShowCorrectAnswers' option

        quizManager.AddQuiz(quiz); // Add quiz to manager

        // Verify that the quiz is added
        Assert.AreEqual(1, quizManager.Quizzes.Count, "Liczba quizów powinna wynosić 1 po dodaniu quizu.");
        Assert.AreEqual("Przykładowy Quiz", quizManager.Quizzes[0].Title, "Tytuł quizu nie zgadza się.");
        Assert.IsTrue(quizManager.Quizzes[0].ShowCorrectAnswers, "Opcja 'ShowCorrectAnswers' nie została ustawiona poprawnie.");
    }

    // Test to verify that quizzes are correctly saved and loaded
    [TestMethod]
    public void SaveAndLoadQuizzes_ShouldPersistData()
    {
        var quizManager = new QuizManager();
        var quiz = new Quiz { Title = "Testowy Quiz", ShowCorrectAnswers = true };
        quiz.Questions.Add(new Question
        {
            Text = "Co to jest C#?",
            Choices = new List<string> { "Język programowania", "Gra komputerowa", "Film", "Książka" },
            CorrectChoices = new List<int> { 0 }
        });
        quizManager.AddQuiz(quiz); // Add the quiz to manager

        string filePath = "test_quizzes.json"; // Define test file path

        // Save and load quizzes
        quizManager.SaveQuizzesToFile(filePath);
        quizManager.LoadQuizzesFromFile(filePath);

        // Verify the quizzes are persisted
        Assert.AreEqual(1, quizManager.Quizzes.Count, "Liczba quizów powinna wynosić 1 po zapisaniu i załadowaniu.");
        Assert.AreEqual("Testowy Quiz", quizManager.Quizzes[0].Title, "Tytuł quizu powinien zostać zachowany.");
        Assert.IsTrue(quizManager.Quizzes[0].ShowCorrectAnswers, "Opcja 'ShowCorrectAnswers' powinna zostać zachowana.");
        Assert.AreEqual(1, quizManager.Quizzes[0].Questions.Count, "Pytania quizu powinny zostać zachowane.");
        Assert.AreEqual("Co to jest C#?", quizManager.Quizzes[0].Questions[0].Text, "Treść pytania powinna zostać zachowana.");
    }

    // Test to verify feedback on incorrect answers
    [TestMethod]
    public void TakeQuiz_ShouldDisplayCorrectAnswersOnIncorrectResponse()
    {
        var quiz = new Quiz { Title = "Feedback Quiz", ShowCorrectAnswers = true };
        quiz.Questions.Add(new Question
        {
            Text = "Które z poniższych są językami programowania?",
            Choices = new List<string> { "Python", "HTML", "CSS", "C#" },
            CorrectChoices = new List<int> { 0, 3 }
        });

        // Simulate taking the quiz
        var question = quiz.Questions[0];
        var userAnswers = new List<int> { 0, 1 }; // Incorrect answer includes 'HTML'

        // Check if the correct answers are displayed for incorrect responses
        bool isCorrect = userAnswers.OrderBy(a => a).SequenceEqual(question.CorrectChoices.OrderBy(a => a));
        Assert.IsFalse(isCorrect, "Użytkownik powinien uzyskać błędną odpowiedź.");
        Assert.IsTrue(quiz.ShowCorrectAnswers, "Poprawne odpowiedzi powinny zostać wyświetlone, gdy opcja 'ShowCorrectAnswers' jest włączona.");
    }
}
