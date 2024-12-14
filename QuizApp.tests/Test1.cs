using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuizApp.Models;
using QuizApp.Persistence;
using QuizApp.Logic;
using System.Collections.Generic;

namespace QuizApp.Tests
{
    [TestClass]
    public class QuizLogicTests
    {
        private QuizLogic _quizLogic = null!; // Initialize to non-nullable to suppress warning
        private QuizManager _quizManager = null!; // Initialize to non-nullable to suppress warning

        [TestInitialize]
        public void Setup()
        {
            _quizManager = new QuizManager("test_quizzes.json");
            _quizLogic = new QuizLogic(_quizManager);
        }

        [TestMethod]
        public void AddQuiz_ShouldAddQuizCorrectly()
        {
            var quiz = new Quiz { Title = "Test Quiz", ShowCorrectAnswers = true };
            _quizManager.AddQuiz(quiz);

            Assert.AreEqual(1, _quizManager.Quizzes.Count);
            Assert.AreEqual("Test Quiz", _quizManager.Quizzes[0].Title);
            Assert.IsTrue(_quizManager.Quizzes[0].ShowCorrectAnswers);
        }

        [TestMethod]
        public void SaveAndLoadQuizzes_ShouldPersistData()
        {
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
            _quizManager.SaveQuizzesToFile();
            _quizManager.LoadQuizzesFromFile();

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

            var question = _quizManager.Quizzes[0].Questions[0];
            question.Text = "What is 10-3?";
            question.Choices = new List<Choice>
            {
                new Choice("6", false),
                new Choice("7", true)
            };

            Assert.AreEqual("What is 10-3?", _quizManager.Quizzes[0].Questions[0].Text);
            Assert.AreEqual(2, _quizManager.Quizzes[0].Questions[0].Choices.Count);
            Assert.AreEqual("7", _quizManager.Quizzes[0].Questions[0].Choices[1].Text);
            Assert.IsTrue(_quizManager.Quizzes[0].Questions[0].Choices[1].IsCorrect);
        }
    }
}
