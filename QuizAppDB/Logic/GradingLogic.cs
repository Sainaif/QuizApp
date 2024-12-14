using QuizAppDB.Models;
using QuizAppDB.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuizAppDB.Logic
{
    public class GradingLogic : QuizLogic
    {
        // Constructor to initialize the GradingLogic with a QuizRepository instance
        public GradingLogic(QuizRepository quizRepository) : base(quizRepository)
        {
        }

        // Override the TakeQuizAsync method to include grading logic
        public override async Task TakeQuizAsync()
        {
            // Retrieve all quizzes from the repository
            var quizzes = await _quizRepository.GetAllQuizzesAsync();
            if (!quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów.");
                return;
            }

            // Display available quizzes
            Console.WriteLine("\nDostępne quizy:");
            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"{quiz.Id}. {quiz.Title}");
            }

            // Prompt user to select a quiz by ID
            Console.Write("\nWybierz ID quizu do rozwiązania: ");
            if (int.TryParse(Console.ReadLine(), out int quizId))
            {
                // Retrieve the selected quiz by ID
                var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
                if (quiz == null)
                {
                    Console.WriteLine("\nNie znaleziono quizu.");
                    return;
                }

                int score = 0;
                // Iterate through each question in the quiz
                foreach (var question in quiz.Questions)
                {
                    Console.WriteLine($"\nPytanie: {question.Text}");

                    // Display choices for the current question
                    for (int i = 0; i < question.Choices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Choices[i].Text}");
                    }

                    // Prompt user to input their answers
                    Console.Write("Twoje odpowiedzi (oddzielone przecinkami): ");
                    string? answersInput = Console.ReadLine();
                    var answers = answersInput?.Split(',').Select(s => int.TryParse(s.Trim(), out int idx) ? idx - 1 : -1).Where(idx => idx >= 0 && idx < question.Choices.Count).ToList();

                    // Check if the user's answers are correct
                    if (answers != null && answers.All(idx => question.Choices[idx].IsCorrect) && answers.Count == question.Choices.Count(c => c.IsCorrect))
                    {
                        score++;
                    }
                    else
                    {
                        Console.WriteLine("Błędna odpowiedź.");
                        // Display correct answers if the quiz setting allows it
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

                // Calculate the percentage score and grade
                int totalQuestions = quiz.Questions.Count;
                double percentage = ((double)score / totalQuestions) * 100;
                int grade = CalculateGrade(percentage);

                // Display the user's score and grade
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
