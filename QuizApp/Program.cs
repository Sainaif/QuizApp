using QuizApp.Logic; // For QuizLogic and GradingLogic
using QuizApp.Persistence; // For QuizManager
using System;

class Program
{
    static void Main()
    {
        // Initialize QuizManager with the path to the quizzes file
        var quizManager = new QuizManager("quizzes.json");

        // Initialize GradingLogic with the QuizManager instance
        var gradingService = new GradingLogic(quizManager); // Grading logic derived from QuizLogic

        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nMenu aplikacji Quiz:");
            Console.WriteLine("1. Stwórz quiz");
            Console.WriteLine("2. Rozwiąż quiz z oceną");
            Console.WriteLine("3. Usuń quiz");
            Console.WriteLine("4. Modyfikuj pytania");
            Console.WriteLine("5. Zapisz quizy do pliku");
            Console.WriteLine("6. Wczytaj quizy z pliku");
            Console.WriteLine("7. Eksportuj quizy do pliku");
            Console.WriteLine("ESC. Wyjdź");
            Console.Write("Wybierz opcję (naciśnij odpowiedni klawisz): ");

            // Read the user's key input
            ConsoleKey key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    // Create a new quiz
                    gradingService.CreateQuiz();
                    PromptContinue();
                    break;
                case ConsoleKey.D2:
                    // Take a quiz with grading
                    gradingService.TakeQuizWithGrading();
                    PromptContinue();
                    break;
                case ConsoleKey.D3:
                    // Delete an existing quiz
                    gradingService.DeleteQuiz();
                    PromptContinue();
                    break;
                case ConsoleKey.D4:
                    // Modify questions in a quiz
                    gradingService.ModifyQuestions();
                    PromptContinue();
                    break;
                case ConsoleKey.D5:
                    // Save quizzes to a file
                    gradingService.SaveQuizzesToFile();
                    PromptContinue();
                    break;
                case ConsoleKey.D6:
                    // Load quizzes from a file
                    Console.Write("\nPodaj ścieżkę do pliku JSON: ");
                    string? importPath = Console.ReadLine();
                    if (!string.IsNullOrEmpty(importPath))
                    {
                        gradingService.LoadQuizzesFromFile(importPath);
                    }
                    else
                    {
                        Console.WriteLine("Ścieżka pliku nie może być pusta!");
                    }
                    PromptContinue();
                    break;
                case ConsoleKey.D7:
                    // Export quizzes to a JSON file
                    Console.Write("\nPodaj ścieżkę do katalogu: ");
                    string? exportDirectory = Console.ReadLine();

                    Console.Write("Podaj nazwę pliku (bez rozszerzenia): ");
                    string? exportFileName = Console.ReadLine();

                    if (!string.IsNullOrEmpty(exportDirectory) && !string.IsNullOrEmpty(exportFileName))
                    {
                        gradingService.ExportQuizzesToJson(exportDirectory, exportFileName);
                    }
                    else
                    {
                        Console.WriteLine("Ścieżka katalogu lub nazwa pliku nie może być pusta!");
                    }
                    PromptContinue();
                    break;
                case ConsoleKey.Escape:
                    // Exit the application
                    Console.WriteLine("\nZamykanie aplikacji...");
                    return; // Exit the program
                default:
                    // Handle invalid input
                    Console.WriteLine("\nNieprawidłowy wybór. Naciśnij dowolny klawisz, aby spróbować ponownie...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // Prompt the user to press any key to continue
    private static void PromptContinue()
    {
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }
}
