﻿using QuizAppDB.Data;
using QuizAppDB.Logic;
using Microsoft.EntityFrameworkCore;
using System;

class Program
{
    static async Task Main()
    {
        // Set up database context and dependencies
        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseSqlite("Data Source=QuizAppDB.sqlite")
            .Options;

        using var dbContext = new QuizDbContext(options);
        dbContext.Database.EnsureCreated(); // Ensure the database is created
        var quizRepository = new QuizRepository(dbContext);
        var gradingService = new GradingLogic(quizRepository); // Use GradingLogic for grading feature

        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nMenu aplikacji Quiz:");
            Console.WriteLine("1. Stwórz quiz");
            Console.WriteLine("2. Rozwiąż quiz (z oceną)");
            Console.WriteLine("3. Usuń quiz");
            Console.WriteLine("4. Modyfikuj pytania");
            Console.WriteLine("5. Importuj quizy z pliku JSON");
            Console.WriteLine("ESC. Wyjdź");
            Console.Write("Wybierz opcję (naciśnij odpowiedni klawisz): ");

            ConsoleKey key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    await gradingService.CreateQuizAsync();
                    PromptContinue();
                    break;
                case ConsoleKey.D2:
                    await gradingService.TakeQuizAsync(); // Includes grading logic
                    PromptContinue();
                    break;
                case ConsoleKey.D3:
                    await gradingService.DeleteQuizAsync();
                    PromptContinue();
                    break;
                case ConsoleKey.D4:
                    await gradingService.ModifyQuestionsAsync();
                    PromptContinue();
                    break;
                case ConsoleKey.D5:
                    Console.Write("\nPodaj ścieżkę do pliku JSON: ");
                    string? filePath = Console.ReadLine();
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        await gradingService.ImportFromJsonAsync(filePath);
                    }
                    else
                    {
                        Console.WriteLine("Ścieżka do pliku nie może być pusta!");
                    }
                    PromptContinue();
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

    private static void PromptContinue()
    {
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }
}
