using QuizApp.Logic; // For QuizLogic
using QuizApp.Persistence; // For QuizManager
using System;

class Program
{
    static QuizLogic quizService = new QuizLogic(new QuizManager("quizzes.json")); // Instantiate the service

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nMenu aplikacji Quiz:");
            Console.WriteLine("1. Stwórz quiz");
            Console.WriteLine("2. Rozwiąż quiz");
            Console.WriteLine("3. Usuń quiz");
            Console.WriteLine("4. Modyfikuj pytania");
            Console.WriteLine("5. Zapisz quizy do pliku");
            Console.WriteLine("6. Wczytaj quizy z pliku");
            Console.WriteLine("ESC. Wyjdź");
            Console.Write("Wybierz opcję (naciśnij odpowiedni klawisz): ");

            ConsoleKey key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    quizService.CreateQuiz();
                    break;
                case ConsoleKey.D2:
                    quizService.TakeQuiz();
                    break;
                case ConsoleKey.D3:
                    quizService.DeleteQuiz();
                    break;
                case ConsoleKey.D4:
                    quizService.ModifyQuestions();
                    break;
                case ConsoleKey.D5:
                    quizService.SaveQuizzes();
                    break;
                case ConsoleKey.D6:
                    quizService.LoadQuizzes();
                    break;
                case ConsoleKey.Escape:
                    return;
                default:
                    Console.WriteLine("\nNieprawidłowy wybór. Naciśnij dowolny klawisz, aby spróbować ponownie...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
