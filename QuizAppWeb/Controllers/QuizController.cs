using Microsoft.AspNetCore.Mvc;
using QuizAppDB.Data;
using QuizAppDB.Models;
using QuizAppDB.Logic;

namespace QuizAppWeb.Controllers
{
    public class QuizController : Controller
    {
        private readonly QuizRepository _quizRepository;

        public QuizController(QuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        // Display the list of quizzes
        public async Task<IActionResult> Index()
        {
            var quizzes = await _quizRepository.GetAllQuizzesAsync();
            return View(quizzes);
        }

        // Display details of a selected quiz
        public async Task<IActionResult> Details(int id)
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            return View(quiz);
        }

        // Handle quiz submission and calculate results
        [HttpPost]
        public async Task<IActionResult> SubmitAnswers(int quizId, Dictionary<int, List<int>> answers)
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
            if (quiz == null)
            {
                return NotFound();
            }

            int score = 0;

            // Create a dictionary to track selected choices for each question
            var questionResults = new Dictionary<int, List<int>>();

            foreach (var question in quiz.Questions)
            {
                // Get user answers and correct answers
                var userAnswers = answers.ContainsKey(question.Id) ? answers[question.Id] : new List<int>();
                var correctChoices = question.Choices.Where(c => c.IsCorrect).Select(c => c.Id).ToList();

                // Track user-selected answers
                questionResults[question.Id] = userAnswers;

                // Calculate score if all user answers are correct and match the correct choices
                if (userAnswers.All(correctChoices.Contains) && userAnswers.Count == correctChoices.Count)
                {
                    score++;
                }
            }

            // Calculate percentage and grade
            double percentage = ((double)score / quiz.Questions.Count) * 100;
            var gradingLogic = new GradingLogic(_quizRepository);
            int grade = gradingLogic.CalculateGrade(percentage);

            // Pass data to the view
            ViewBag.Score = $"{score}/{quiz.Questions.Count}";
            ViewBag.Percentage = percentage.ToString("F2");
            ViewBag.Grade = grade;
            ViewBag.QuestionResults = questionResults;

            return View("Results", quiz);
        }
    }
}
