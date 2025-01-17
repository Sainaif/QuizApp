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

        public async Task<IActionResult> Index()
        {
            var quizzes = await _quizRepository.GetAllQuizzesAsync();
            return View(quizzes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            return View(quiz);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswers(int quizId, Dictionary<int, List<int>> answers)
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
            if (quiz == null)
            {
                return NotFound();
            }

            int score = 0;
            foreach (var question in quiz.Questions)
            {
                var userAnswers = answers.ContainsKey(question.Id) ? answers[question.Id] : new List<int>();
                var correctChoices = question.Choices.Where(c => c.IsCorrect).Select(c => c.Id).ToList();

                if (userAnswers.All(correctChoices.Contains) && userAnswers.Count == correctChoices.Count)
                {
                    score++;
                }
            }

            double percentage = ((double)score / quiz.Questions.Count) * 100;

            // Tworzenie instancji GradingLogic i obliczenie oceny
            var gradingLogic = new GradingLogic(_quizRepository);
            int grade = gradingLogic.CalculateGrade(percentage);

            ViewBag.Score = $"{score}/{quiz.Questions.Count}";
            ViewBag.Grade = grade;

            return View("Results", quiz);
        }
    }
}
