// QuizLogic.cs
using QuizApp.Models; // Importing models for Quiz and Question
using QuizApp.Persistence; // Importing persistence layer for managing quizzes
using System; // Importing system namespace for console operations
using System.Linq; // Importing LINQ for collection manipulation

namespace QuizApp.Logic
{
    public class QuizLogic
    {
        private readonly QuizManager _quizManager; // Dependency on QuizManager for managing quizzes

        public QuizLogic(QuizManager quizManager)
        {
            _quizManager = quizManager;
        }

        // Method to create a new quiz
        public void CreateQuiz()
        {
            Console.Write("\nPodaj tytuł quizu: ");
            string? title = Console.ReadLine();
            if (string.IsNullOrEmpty(title))
            {
                Console.WriteLine("\nTytuł quizu nie może być pusty!");
                Console.ReadKey();
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
                        question.Choices.Add(new Choice(choiceText ?? string.Empty, isCorrect));
                    }

                    quiz.Questions.Add(question);
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa liczba odpowiedzi. Pytanie pominięte.");
                }
            }

            _quizManager.AddQuiz(quiz);
            Console.WriteLine("\nQuiz utworzony! Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }

        // Method to take an existing quiz
        public void TakeQuiz()
        {
            if (!_quizManager.Quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nDostępne quizy:");
            for (int i = 0; i < _quizManager.Quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_quizManager.Quizzes[i].Title}");
            }

            Console.Write("\nWybierz numer quizu do rozwiązania: ");
            if (int.TryParse(Console.ReadLine(), out int quizIndex) && quizIndex >= 1 && quizIndex <= _quizManager.Quizzes.Count)
            {
                var quiz = _quizManager.Quizzes[quizIndex - 1];
                int score = 0;

                foreach (var question in quiz.Questions)
                {
                    Console.WriteLine($"\n{question.Text}");
                    for (int i = 0; i < question.Choices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Choices[i].Text}");
                    }

                    Console.Write("Twoje odpowiedzi (oddzielone przecinkami): ");
                    string? answersInput = Console.ReadLine();
                    var answers = answersInput?.Split(',').Select(s => int.TryParse(s.Trim(), out int idx) ? idx - 1 : -1).Where(idx => idx >= 0 && idx < question.Choices.Count).ToList();

                    if (answers != null && answers.All(idx => question.Choices[idx].IsCorrect) && answers.Count == question.GetCorrectChoices().Count)
                    {
                        score++;
                    }
                    else
                    {
                        Console.WriteLine("Błędna odpowiedź.");
                        if (quiz.ShowCorrectAnswers)
                        {
                            Console.WriteLine("Poprawne odpowiedzi:");
                            foreach (var correct in question.GetCorrectChoices())
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
                Console.WriteLine("\nNieprawidłowy numer quizu.");
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }

        // Method to delete a quiz
        public void DeleteQuiz()
        {
            if (!_quizManager.Quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów do usunięcia.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nDostępne quizy:");
            for (int i = 0; i < _quizManager.Quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_quizManager.Quizzes[i].Title}");
            }

            Console.Write("\nWybierz numer quizu do usunięcia: ");
            if (int.TryParse(Console.ReadLine(), out int quizIndex) && quizIndex >= 1 && quizIndex <= _quizManager.Quizzes.Count)
            {
                var deletedQuiz = _quizManager.Quizzes[quizIndex - 1];
                _quizManager.Quizzes.RemoveAt(quizIndex - 1);
                Console.WriteLine($"\nQuiz '{deletedQuiz.Title}' został usunięty.");
            }
            else
            {
                Console.WriteLine("\nNieprawidłowy numer quizu.");
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }

        // Method to modify questions in a quiz
        public void ModifyQuestions()
        {
            if (!_quizManager.Quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów do modyfikacji.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nDostępne quizy:");
            for (int i = 0; i < _quizManager.Quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_quizManager.Quizzes[i].Title}");
            }

            Console.Write("\nWybierz numer quizu do modyfikacji: ");
            if (int.TryParse(Console.ReadLine(), out int quizIndex) && quizIndex >= 1 && quizIndex <= _quizManager.Quizzes.Count)
            {
                var quiz = _quizManager.Quizzes[quizIndex - 1];

                Console.WriteLine("\nPytania w quizie:");
                for (int i = 0; i < quiz.Questions.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {quiz.Questions[i].Text}");
                }

                Console.Write("\nWybierz numer pytania do modyfikacji: ");
                if (int.TryParse(Console.ReadLine(), out int questionIndex) && questionIndex >= 1 && questionIndex <= quiz.Questions.Count)
                {
                    var question = quiz.Questions[questionIndex - 1];

                    Console.WriteLine($"\nAktualne pytanie: {question.Text}");
                    Console.Write("Podaj nową treść pytania (pozostaw puste, aby nie zmieniać): ");
                    string? newQuestionText = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newQuestionText))
                    {
                        question.Text = newQuestionText;
                    }

                    Console.WriteLine("\nAktualne odpowiedzi:");
                    for (int i = 0; i < question.Choices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Choices[i].Text}");
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
                                question.Choices.Add(new Choice(choiceText ?? string.Empty, isCorrect));
                            }
                        }
                    }

                    Console.WriteLine("\nPytanie zostało zaktualizowane! Naciśnij dowolny klawisz, aby kontynuować...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("\nNieprawidłowy numer pytania.");
                }
            }
            else
            {
                Console.WriteLine("\nNieprawidłowy numer quizu.");
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }

        // Saves all quizzes to file
        public void SaveQuizzes()
        {
            _quizManager.SaveQuizzesToFile();
            Console.WriteLine("\nQuizy zostały zapisane do pliku!");
            Console.ReadKey();
        }

        // Loads all quizzes from file
        public void LoadQuizzes()
        {
            _quizManager.LoadQuizzesFromFile();
            Console.WriteLine("\nQuizy zostały wczytane z pliku!");
            Console.ReadKey();
        }
    }
}

