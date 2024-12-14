using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuizAppDB.Data;
using QuizAppDB.Models;
using QuizAppDB.Logic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuizAppDB.Tests
{
    [TestClass]
    public class QuizLogicTests
    {
        private string _databasePath = null!;
        private QuizDbContext _context = null!;
        private QuizRepository _repository = null!;
        private QuizLogic _logic = null!;

        [TestInitialize]
        public void Setup()
        {
            // Set a unique database file for the test
            _databasePath = Path.Combine(Path.GetTempPath(), $"TestQuizDb_{Guid.NewGuid()}.sqlite");

            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseSqlite($"Data Source={_databasePath}")
                .Options;

            _context = new QuizDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new QuizRepository(_context);
            _logic = new QuizLogic(_repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose DbContext to release database locks
            _context.Dispose();

            // Retry deleting the database file
            if (File.Exists(_databasePath))
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        File.Delete(_databasePath);
                        break; // Exit loop if successful
                    }
                    catch (IOException)
                    {
                        Task.Delay(100).Wait(); // Wait and retry
                    }
                }
            }
        }

        [TestMethod]
        public async Task AddQuiz_ShouldAddQuizCorrectly()
        {
            // Arrange
            var quiz = new Quiz { Title = "Test Quiz", ShowCorrectAnswers = true };

            // Act
            await _repository.AddQuizAsync(quiz);
            var quizzes = await _repository.GetAllQuizzesAsync();

            // Assert
            Assert.AreEqual(1, quizzes.Count, "Expected exactly 1 quiz in the database.");
            Assert.AreEqual("Test Quiz", quizzes[0].Title);
            Assert.IsTrue(quizzes[0].ShowCorrectAnswers);
        }

        [TestMethod]
        public async Task SaveAndLoadQuizzes_ShouldPersistData()
        {
            // Arrange
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
                            new Choice { Text = "3", IsCorrect = false },
                            new Choice { Text = "4", IsCorrect = true }
                        }
                    }
                }
            };

            // Act
            await _repository.AddQuizAsync(quiz);
            var quizzes = await _repository.GetAllQuizzesAsync();

            // Assert
            Assert.AreEqual(1, quizzes.Count, "Expected exactly 1 quiz in the database.");
            Assert.AreEqual("Sample Quiz", quizzes[0].Title);
            Assert.AreEqual(1, quizzes[0].Questions.Count);
            Assert.AreEqual("What is 2+2?", quizzes[0].Questions[0].Text);
            Assert.AreEqual(2, quizzes[0].Questions[0].Choices.Count);
            Assert.IsTrue(quizzes[0].Questions[0].Choices.Any(c => c.Text == "4" && c.IsCorrect));
        }

        [TestMethod]
        public async Task ModifyQuestions_ShouldUpdateQuestionCorrectly()
        {
            // Arrange
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
                            new Choice { Text = "7", IsCorrect = false },
                            new Choice { Text = "8", IsCorrect = true }
                        }
                    }
                }
            };

            await _repository.AddQuizAsync(quiz);

            var savedQuiz = (await _repository.GetAllQuizzesAsync()).FirstOrDefault();
            Assert.IsNotNull(savedQuiz, "No quiz found after adding.");

            var question = savedQuiz.Questions.FirstOrDefault();
            Assert.IsNotNull(question, "No questions found in the quiz.");

            // Act
            question.Text = "What is 10-3?";
            question.Choices = new List<Choice>
            {
                new Choice { Text = "6", IsCorrect = false },
                new Choice { Text = "7", IsCorrect = true }
            };

            await _repository.UpdateQuizAsync(savedQuiz);

            // Assert
            var updatedQuiz = (await _repository.GetAllQuizzesAsync()).First();
            Assert.AreEqual("What is 10-3?", updatedQuiz.Questions.First().Text);
            Assert.AreEqual(2, updatedQuiz.Questions.First().Choices.Count);
            Assert.AreEqual("7", updatedQuiz.Questions.First().Choices.Last().Text);
            Assert.IsTrue(updatedQuiz.Questions.First().Choices.Last().IsCorrect);
        }
    }
}
