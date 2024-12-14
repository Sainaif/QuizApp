using QuizApp.Models;
using QuizApp.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizApp.Logic
{
    public class QuizLogic
    {
        // Protected field to store the QuizManager instance
        protected readonly QuizManager _quizManager;

        // Constructor to initialize the QuizLogic with a QuizManager instance
        public QuizLogic(QuizManager quizManager)
        {
            _quizManager = quizManager;
        }

        // Method to create a new quiz
        public void CreateQuiz()
        {
            // Prompt the user for the quiz title
            Console.Write("\nPodaj tytuł quizu: ");
            string? title = Console.ReadLine();
            if (string.IsNullOrEmpty(title))
            {
                Console.WriteLine("\nTytuł quizu nie może być pusty!");
                return;
            }

            // Ask if the user wants to show correct answers after incorrect attempts
            Console.Write("\nCzy chcesz pokazać poprawne odpowiedzi po błędnych odpowiedziach? (tak/nie): ");
            bool showCorrectAnswers = Console.ReadLine()?.ToLower() == "tak";

            // Create a new quiz instance
            var quiz = new Quiz { Title = title, ShowCorrectAnswers = showCorrectAnswers };

            // Loop to add questions to the quiz
            while (true)
            {
                Console.Write("\nPodaj treść pytania (lub wpisz 'koniec', aby zakończyć): ");
                string? questionText = Console.ReadLine();
                if (string.IsNullOrEmpty(questionText) || questionText.ToLower() == "koniec")
                    break;

                var question = new Question { Text = questionText };

                // Prompt the user for the number of choices for the question
                Console.Write("Podaj liczbę odpowiedzi dla tego pytania: ");
                if (int.TryParse(Console.ReadLine(), out int numChoices) && numChoices > 1)
                {
                    // Loop to add choices to the question
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

            // Add the created quiz to the quiz manager
            _quizManager.AddQuiz(quiz);
            Console.WriteLine("\nQuiz utworzony!");
        }

        // Method to save quizzes to a file
        public void SaveQuizzesToFile()
        {
            // Prompt the user for the directory path to save the quizzes
            Console.Write("\nPodaj ścieżkę do katalogu, w którym zapisać quizy: ");
            string? directoryPath = Console.ReadLine();

            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                Console.WriteLine("Podano nieprawidłową ścieżkę katalogu!");
                return;
            }

            // Prompt the user for the file name
            Console.Write("\nPodaj nazwę pliku (bez rozszerzenia): ");
            string? fileName = Console.ReadLine();

            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Nazwa pliku nie może być pusta!");
                return;
            }

            string filePath = Path.Combine(directoryPath, fileName + ".json");

            try
            {
                // Serialize quizzes to JSON and write to file
                string jsonData = JsonSerializer.Serialize(_quizManager.Quizzes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonData);
                Console.WriteLine($"\nQuizy zostały pomyślnie zapisane w pliku: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nWystąpił błąd podczas zapisywania quizów: {ex.Message}");
            }
        }

        // Method to load quizzes from a file
        public void LoadQuizzesFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("\nPodano nieprawidłową ścieżkę lub plik nie istnieje!");
                return;
            }

            try
            {
                // Read JSON data from file and deserialize to list of quizzes
                string jsonData = File.ReadAllText(filePath);
                var quizzes = JsonSerializer.Deserialize<List<Quiz>>(jsonData);

                if (quizzes != null)
                {
                    // Add each quiz to the quiz manager
                    foreach (var quiz in quizzes)
                    {
                        _quizManager.AddQuiz(quiz);
                    }
                    Console.WriteLine("\nQuizy zostały pomyślnie wczytane!");
                }
                else
                {
                    Console.WriteLine("\nPlik nie zawiera poprawnych danych quizów.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nWystąpił błąd podczas wczytywania quizów: {ex.Message}");
            }
        }

        // Method to delete a quiz
        public void DeleteQuiz()
        {
            if (!_quizManager.Quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów do usunięcia.");
                return;
            }

            // Display available quizzes
            Console.WriteLine("\nDostępne quizy:");
            for (int i = 0; i < _quizManager.Quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_quizManager.Quizzes[i].Title}");
            }

            // Prompt the user to select a quiz to delete
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
        }

        // Method to modify questions in a quiz
        public void ModifyQuestions()
        {
            if (!_quizManager.Quizzes.Any())
            {
                Console.WriteLine("\nBrak dostępnych quizów do modyfikacji.");
                return;
            }

            // Display available quizzes
            Console.WriteLine("\nDostępne quizy:");
            for (int i = 0; i < _quizManager.Quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_quizManager.Quizzes[i].Title}");
            }

            // Prompt the user to select a quiz to modify
            Console.Write("\nWybierz numer quizu do modyfikacji: ");
            if (int.TryParse(Console.ReadLine(), out int quizIndex) && quizIndex >= 1 && quizIndex <= _quizManager.Quizzes.Count)
            {
                var quiz = _quizManager.Quizzes[quizIndex - 1];

                // Display questions in the selected quiz
                Console.WriteLine("\nPytania w quizie:");
                for (int i = 0; i < quiz.Questions.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {quiz.Questions[i].Text}");
                }

                // Prompt the user to select a question to modify
                Console.Write("\nWybierz numer pytania do modyfikacji: ");
                if (int.TryParse(Console.ReadLine(), out int questionIndex) && questionIndex >= 1 && questionIndex <= quiz.Questions.Count)
                {
                    var question = quiz.Questions[questionIndex - 1];

                    // Display current question text and prompt for new text
                    Console.WriteLine($"\nAktualne pytanie: {question.Text}");
                    Console.Write("Podaj nową treść pytania (pozostaw puste, aby nie zmieniać): ");
                    string? newQuestionText = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newQuestionText))
                    {
                        question.Text = newQuestionText;
                    }

                    // Display current choices and prompt to change them
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
                            // Loop to add new choices to the question
                            for (int i = 1; i <= numChoices; i++)
                            {
                                Console.Write($"Podaj odpowiedź {i}: ");
                                string? choiceText = Console.ReadLine();
                                Console.Write("Czy ta odpowiedź jest poprawna? (tak/nie): ");
                                bool isCorrect = Console.ReadLine()?.ToLower() == "tak";
                                question.Choices.Add(new Choice(choiceText ?? string.Empty, isCorrect));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowa liczba odpowiedzi. Zmiana odpowiedzi anulowana.");
                        }
                    }
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
        }

        // Method to export quizzes to a JSON file
        public void ExportQuizzesToJson(string directoryPath, string fileName)
        {
            // Validate the directory path
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                Console.WriteLine("Podano nieprawidłową ścieżkę katalogu!");
                return;
            }

            // Validate the file name
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Nazwa pliku nie może być pusta!");
                return;
            }

            string filePath = Path.Combine(directoryPath, fileName + ".json");

            try
            {
                // Serialize quizzes to JSON and write to file
                string jsonData = JsonSerializer.Serialize(_quizManager.Quizzes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonData);
                Console.WriteLine($"\nQuizy zostały pomyślnie zapisane w pliku: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nWystąpił błąd podczas zapisywania quizów: {ex.Message}");
            }
        }
    }
}
