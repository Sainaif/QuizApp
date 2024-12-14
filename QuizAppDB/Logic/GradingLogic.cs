using QuizAppDB.Models;
using QuizAppDB.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuizAppDB.Logic
{
    public class GradingLogic : QuizLogic
    {
        public GradingLogic(QuizRepository quizRepository) : base(quizRepository)
        {
        }

        // Override the TakeQuizAsync method to include grading logic
        public override async Task TakeQuizAsync()
        {
            var quizzes = await _quizRepository.GetAllQuizzesAsync();
            if (!quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów.");
                return;
            }

            Console.WriteLine("\nDostępne quizy:");
            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"{quiz.Id}. {quiz.Title}");
            }

            Console.Write("\nWybierz ID quizu do rozwiązania: ");
            if (int.TryParse(Console.ReadLine(), out int quizId))
            {
                var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
                if (quiz == null)
                {
                    Console.WriteLine("\nNie znaleziono quizu.");
                    return;
                }

                int score = 0;
                foreach (var question in quiz.Questions)
                {
                    Console.WriteLine($"\nPytanie: {question.Text}");

                    for (int i = 0; i < question.Choices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Choices[i].Text}");
                    }

                    Console.Write("Twoje odpowiedzi (oddzielone przecinkami): ");
                    string? answersInput = Console.ReadLine();
                    var answers = answersInput?.Split(',').Select(s => int.TryParse(s.Trim(), out int idx) ? idx - 1 : -1).Where(idx => idx >= 0 && idx < question.Choices.Count).ToList();

                    if (answers != null && answers.All(idx => question.Choices[idx].IsCorrect) && answers.Count == question.Choices.Count(c => c.IsCorrect))
                    {
                        score++;
                    }
                    else
                    {
                        Console.WriteLine("Błędna odpowiedź.");
                        if (quiz.ShowCorrectAnswers)
                        {
                            Console.WriteLine("Poprawne odpowiedzi:");
                            foreach (var correct in question.Choices.Where(c => c.IsCorrect))
                            {
                                Console.WriteLine($"- {correct.Text}");
                            }
                        }
                    }
                }

                int totalQuestions = quiz.Questions.Count;
                double percentage = ((double)score / totalQuestions) * 100;
                int grade = CalculateGrade(percentage);

                Console.WriteLine($"\nTwój wynik: {score}/{totalQuestions} ({percentage:F2}%)!");
                Console.WriteLine($"Ocena: {grade}");
            }
            else
            {
                Console.WriteLine("\nNieprawidłowy ID quizu.");
            }
        }

        // Method to calculate the grade based on percentage
        private int CalculateGrade(double percentage)
        {
            if (percentage >= 90)
                return 5;
            if (percentage >= 75)
                return 4;
            if (percentage >= 50)
                return 3;
            return 2;
        }
    }
}
