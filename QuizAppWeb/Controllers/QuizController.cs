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
                // Handle cases where no answer is provided
                var userAnswers = answers.ContainsKey(question.Id) ? answers[question.Id] : new List<int>();

                // Track user-selected answers (empty if none provided)
                questionResults[question.Id] = userAnswers;

                // Check correctness
                var correctChoices = question.Choices.Where(c => c.IsCorrect).Select(c => c.Id).ToList();

                // If no answers are selected, mark the question as incorrect
                if (userAnswers.Count == 0 || !userAnswers.All(correctChoices.Contains) || userAnswers.Count != correctChoices.Count)
                {
                    continue; // Skip incrementing the score for incorrect or unanswered questions
                }

                // Increment score if all answers are correct
                score++;
            }

            // Calculate grade
            var gradingLogic = new GradingLogic(_quizRepository);
            int grade = gradingLogic.CalculateGrade(((double)score / quiz.Questions.Count) * 100);

            // Pass data to the view
            ViewBag.Score = $"{score}/{quiz.Questions.Count}";
            ViewBag.Grade = grade;
            ViewBag.QuestionResults = questionResults;
            ViewBag.ShowCorrectAnswers = quiz.ShowCorrectAnswers; // Boolean directly passed to ViewBag

            return View("Results", quiz);
        }
    }
}
