using QuizApp.Models;
using QuizApp.Persistence;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace QuizApp.Logic
{
    public class GradingLogic : QuizLogic
    {
        // Constructor that initializes the GradingLogic with a QuizManager instance
        public GradingLogic(QuizManager quizManager) : base(quizManager)
        {
        }

        // Method to take a quiz and grade it
        public void TakeQuizWithGrading()
        {
            // Check if there are any quizzes available
            if (!_quizManager.Quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów.");
                return;
            }

            // Display available quizzes
            Console.WriteLine("\nDostępne quizy:");
            for (int i = 0; i < _quizManager.Quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_quizManager.Quizzes[i].Title}");
            }

            // Prompt user to select a quiz
            Console.Write("\nWybierz numer quizu do rozwiązania: ");
            if (int.TryParse(Console.ReadLine(), out int quizIndex) && quizIndex > 0 && quizIndex <= _quizManager.Quizzes.Count)
            {
                var quiz = _quizManager.Quizzes[quizIndex - 1];
                int correctAnswers = 0;

                // Iterate through each question in the selected quiz
                foreach (var question in quiz.Questions)
                {
                    Console.WriteLine($"\nPytanie: {question.Text}");
                    for (int i = 0; i < question.Choices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Choices[i].Text}");
                    }

                    // Prompt user to enter their answers
                    Console.Write("Twoje odpowiedzi (oddzielone przecinkami): ");
                    string? answersInput = Console.ReadLine();
                    var selectedAnswers = answersInput?.Split(',').Select(s => int.TryParse(s.Trim(), out int idx) ? idx - 1 : -1).Where(idx => idx >= 0 && idx < question.Choices.Count).ToList();

                    // Check if the selected answers are correct
                    if (selectedAnswers != null && selectedAnswers.All(idx => question.Choices[idx].IsCorrect) && selectedAnswers.Count == question.Choices.Count(c => c.IsCorrect))
                    {
                        correctAnswers++;
                    }
                }

                // Calculate the percentage of correct answers
                double percentage = (double)correctAnswers / quiz.Questions.Count * 100;
                int grade = CalculateGrade(percentage);

                // Display the result and grade
                Console.WriteLine($"\nTwój wynik: {correctAnswers}/{quiz.Questions.Count} ({percentage:F2}%)");
                Console.WriteLine($"Ocena: {grade}");
            }
            else
            {
                Console.WriteLine("\nNieprawidłowy numer quizu.");
            }
        }

        // Method to calculate the grade based on the percentage of correct answers
        private int CalculateGrade(double percentage)
        {
            if (percentage >= 90) return 5; // Grade 5 for 90% and above
            if (percentage >= 75) return 4; // Grade 4 for 75% to 89%
            if (percentage >= 50) return 3; // Grade 3 for 50% to 74%
            return 2; // Grade 2 for below 50%
        }
    }
}
