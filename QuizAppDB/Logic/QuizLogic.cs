using QuizAppDB.Models;
using QuizAppDB.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuizAppDB.Logic
{
    public class QuizLogic
    {
        private readonly QuizRepository _quizRepository; // Dependency for database operations

        public QuizLogic(QuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        // Method to create a new quiz
        public async Task CreateQuizAsync()
        {
            Console.Write("\nPodaj tytuł quizu: ");
            string? title = Console.ReadLine();
            if (string.IsNullOrEmpty(title))
            {
                Console.WriteLine("\nTytuł quizu nie może być pusty!");
                return;
            }

            Console.Write("\nCzy chcesz pokazać poprawne odpowiedzi po błędnych odpowiedziach? (tak/nie): ");
            bool showCorrectAnswers = Console.ReadLine()?.ToLower() == "tak";

            var quiz = new Quiz { Title = title, ShowCorrectAnswers = showCorrectAnswers };

            while (true)
            {
                Console.Write("\nPodaj treść pytania (lub wpisz 'koniec', aby zakończyć): ");
                string? questionText = Console.ReadLine();
                if (string.IsNullOrEmpty(questionText) || questionText.ToLower() == "koniec")
                    break;

                var question = new Question { Text = questionText };

                Console.Write("Podaj liczbę odpowiedzi dla tego pytania: ");
                if (int.TryParse(Console.ReadLine(), out int numChoices) && numChoices > 1)
                {
                    for (int i = 1; i <= numChoices; i++)
                    {
                        Console.Write($"Podaj odpowiedź {i}: ");
                        string? choiceText = Console.ReadLine();
                        Console.Write("Czy ta odpowiedź jest poprawna? (tak/nie): ");
                        bool isCorrect = Console.ReadLine()?.ToLower() == "tak";
                        question.Choices.Add(new Choice { Text = choiceText ?? string.Empty, IsCorrect = isCorrect });
                    }

                    quiz.Questions.Add(question);
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa liczba odpowiedzi. Pytanie pominięte.");
                }
            }

            await _quizRepository.AddQuizAsync(quiz);
            Console.WriteLine("\nQuiz utworzony!\n");
        }

        // Method to take an existing quiz
        public async Task TakeQuizAsync()
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

                Console.WriteLine($"\nTwój wynik: {score}/{quiz.Questions.Count}!");
            }
            else
            {
                Console.WriteLine("\nNieprawidłowy ID quizu.");
            }
        }

        // Method to delete a quiz
        public async Task DeleteQuizAsync()
        {
            var quizzes = await _quizRepository.GetAllQuizzesAsync();
            if (!quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów do usunięcia.");
                return;
            }

            Console.WriteLine("\nDostępne quizy:");
            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"{quiz.Id}. {quiz.Title}");
            }

            Console.Write("\nWybierz ID quizu do usunięcia: ");
            if (int.TryParse(Console.ReadLine(), out int quizId))
            {
                await _quizRepository.DeleteQuizAsync(quizId);
                Console.WriteLine("\nQuiz został usunięty!");
            }
            else
            {
                Console.WriteLine("\nNieprawidłowy ID quizu.");
            }
        }

        // Method to modify questions in a quiz
        public async Task ModifyQuestionsAsync()
        {
            var quizzes = await _quizRepository.GetAllQuizzesAsync();
            if (!quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów do modyfikacji.");
                return;
            }

            Console.WriteLine("\nDostępne quizy:");
            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"{quiz.Id}. {quiz.Title}");
            }

            Console.Write("\nWybierz ID quizu do modyfikacji: ");
            if (int.TryParse(Console.ReadLine(), out int quizId))
            {
                var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
                if (quiz == null)
                {
                    Console.WriteLine("\nNie znaleziono quizu.");
                    return;
                }

                Console.WriteLine("\nPytania w quizie:");
                foreach (var question in quiz.Questions)
                {
                    Console.WriteLine($"{question.Id}. {question.Text}");
                }

                Console.Write("\nPodaj ID pytania do modyfikacji: ");
                if (int.TryParse(Console.ReadLine(), out int questionId))
                {
                    var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId);
                    if (question == null)
                    {
                        Console.WriteLine("\nNie znaleziono pytania.");
                        return;
                    }

                    Console.WriteLine($"\nAktualne pytanie: {question.Text}");
                    Console.Write("Podaj nową treść pytania (pozostaw puste, aby nie zmieniać): ");
                    string? newQuestionText = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newQuestionText))
                    {
                        question.Text = newQuestionText;
                    }

                    Console.WriteLine("\nAktualne odpowiedzi:");
                    foreach (var choice in question.Choices)
                    {
                        Console.WriteLine($"{choice.Id}. {choice.Text}");
                    }

                    Console.Write("Czy chcesz zmienić odpowiedzi? (tak/nie): ");
                    if (Console.ReadLine()?.ToLower() == "tak")
                    {
                        question.Choices.Clear();
                        Console.Write("Podaj liczbę nowych odpowiedzi: ");
                        if (int.TryParse(Console.ReadLine(), out int numChoices) && numChoices > 1)
                        {
                            for (int i = 1; i <= numChoices; i++)
                            {
                                Console.Write($"Podaj odpowiedź {i}: ");
                                string? choiceText = Console.ReadLine();
                                Console.Write("Czy ta odpowiedź jest poprawna? (tak/nie): ");
                                bool isCorrect = Console.ReadLine()?.ToLower() == "tak";
                                question.Choices.Add(new Choice { Text = choiceText ?? string.Empty, IsCorrect = isCorrect });
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowa liczba odpowiedzi. Zmiana odpowiedzi anulowana.");
                        }
                    }

                    await _quizRepository.UpdateQuizAsync(quiz);
                    Console.WriteLine("\nPytanie zostało zaktualizowane!");
                }
                else
                {
                    Console.WriteLine("\nNieprawidłowy ID pytania.");
                }
            }
            else
            {
                Console.WriteLine("\nNieprawidłowy ID quizu.");
            }
        }
    }
}

