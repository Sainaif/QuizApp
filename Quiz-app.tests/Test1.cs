using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace QuizApp.Tests
{
    [TestClass]
    public class QuizManagerTests
    {
        [TestMethod]
        public void AddQuiz_ShouldAddQuizToList()
        {
            // Arrange
            var quizManager = new QuizManager();
            var quiz = new Quiz { Title = "Sample Quiz" };

            // Act
            quizManager.AddQuiz(quiz);

            // Assert
            Assert.AreEqual(1, quizManager.Quizzes.Count);
            Assert.AreEqual("Sample Quiz", quizManager.Quizzes[0].Title);
        }

        [TestMethod]
        public void SaveAndLoadQuizzes_ShouldPersistData()
        {
            // Arrange
            var quizManager = new QuizManager();
            var quiz = new Quiz { Title = "Test Quiz" };
            quiz.Questions.Add(new Question
            {
                Text = "What is 2+2?",
                Choices = new List<string> { "3", "4" },
                CorrectChoiceIndex = 1
            });
            quizManager.AddQuiz(quiz);

            string filePath = "test_quizzes.json";

            // Act
            quizManager.SaveQuizzesToFile(filePath);
            quizManager.LoadQuizzesFromFile(filePath);

            // Assert
            Assert.AreEqual(1, quizManager.Quizzes.Count);
            Assert.AreEqual("Test Quiz", quizManager.Quizzes[0].Title);
        }
    }
}
