using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuizApp.Models;
using QuizApp.Persistence;
using QuizApp.Logic;
using System.Collections.Generic;
using System.IO;

namespace QuizApp.Tests
{
    [TestClass]
    public class QuizLogicTests
    {
        private QuizLogic _quizLogic = null!; // Initialize to non-nullable to suppress warnings
        private QuizManager _quizManager = null!; // Initialize to non-nullable to suppress warnings
        private string _testFilePath = null!; // Path for testing file operations

        [TestInitialize]
        public void Setup()
        {
            // Set up the test file path and initialize QuizManager and QuizLogic
            _testFilePath = Path.Combine(Path.GetTempPath(), "test_quizzes.json");
            _quizManager = new QuizManager(_testFilePath);
            _quizLogic = new QuizLogic(_quizManager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up the test file after each test
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestMethod]
        public void AddQuiz_ShouldAddQuizCorrectly()
        {
            // Arrange: Create a new quiz
            var quiz = new Quiz { Title = "Test Quiz", ShowCorrectAnswers = true };

            // Act: Add the quiz to the manager
            _quizManager.AddQuiz(quiz);

            // Assert: Verify the quiz was added correctly
            Assert.AreEqual(1, _quizManager.Quizzes.Count);
            Assert.AreEqual("Test Quiz", _quizManager.Quizzes[0].Title);
            Assert.IsTrue(_quizManager.Quizzes[0].ShowCorrectAnswers);
        }

        [TestMethod]
        public void SaveAndLoadQuizzes_ShouldPersistData()
        {
            // Arrange: Create a new quiz with questions and choices
            var quiz = new Quiz
            {
                Title = "Sample Quiz",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "What is 2+2?",
                        Choices = new List<Choice>
                        {
                            new Choice("3", false),
                            new Choice("4", true)
                        }
                    }
                }
            };

            _quizManager.AddQuiz(quiz);

            // Act: Save the quizzes to a file and then load them back
            _quizManager.SaveQuizzesToFile();
            _quizManager.LoadQuizzesFromFile();

            // Assert: Verify the quizzes were persisted correctly
            Assert.AreEqual(1, _quizManager.Quizzes.Count);
            Assert.AreEqual("Sample Quiz", _quizManager.Quizzes[0].Title);
            Assert.AreEqual(1, _quizManager.Quizzes[0].Questions.Count);
            Assert.AreEqual("What is 2+2?", _quizManager.Quizzes[0].Questions[0].Text);
            Assert.AreEqual(2, _quizManager.Quizzes[0].Questions[0].Choices.Count);
            Assert.IsTrue(_quizManager.Quizzes[0].Questions[0].Choices[1].IsCorrect);
        }

        [TestMethod]
        public void ModifyQuestions_ShouldUpdateQuestionCorrectly()
        {
            // Arrange: Create a new quiz with a question
            var quiz = new Quiz
            {
                Title = "Math Quiz",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "What is 3+5?",
                        Choices = new List<Choice>
                        {
                            new Choice("7", false),
                            new Choice("8", true)
                        }
                    }
                }
            };

            _quizManager.AddQuiz(quiz);

            // Act: Modify the question text and choices
            var question = _quizManager.Quizzes[0].Questions[0];
            question.Text = "What is 10-3?";
            question.Choices = new List<Choice>
            {
                new Choice("6", false),
                new Choice("7", true)
            };

            // Assert: Verify the question was updated correctly
            Assert.AreEqual("What is 10-3?", _quizManager.Quizzes[0].Questions[0].Text);
            Assert.AreEqual(2, _quizManager.Quizzes[0].Questions[0].Choices.Count);
            Assert.AreEqual("7", _quizManager.Quizzes[0].Questions[0].Choices[1].Text);
            Assert.IsTrue(_quizManager.Quizzes[0].Questions[0].Choices[1].IsCorrect);
        }

        [TestMethod]
        public void ExportQuizzesToJson_ShouldCreateJsonFile()
        {
            // Arrange: Create a new quiz
            var quiz = new Quiz
            {
                Title = "Export Test Quiz",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Sample Question?",
                        Choices = new List<Choice>
                        {
                            new Choice("Option 1", true),
                            new Choice("Option 2", false)
                        }
                    }
                }
            };

            _quizManager.AddQuiz(quiz);

            // Act: Export the quizzes to a JSON file
            string exportDirectory = Path.GetTempPath();
            string fileName = "test_export_quizzes";
            _quizLogic.ExportQuizzesToJson(exportDirectory, fileName);

            string expectedFilePath = Path.Combine(exportDirectory, fileName + ".json");

            // Assert: Verify the JSON file was created and contains the correct data
            Assert.IsTrue(File.Exists(expectedFilePath));

            var jsonData = File.ReadAllText(expectedFilePath);
            Assert.IsTrue(jsonData.Contains("Export Test Quiz"));
            Assert.IsTrue(jsonData.Contains("Sample Question?"));

            // Cleanup: Delete the exported JSON file
            File.Delete(expectedFilePath);
        }
    }
}
