// Program.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizAppDB.Data;
using QuizAppDB.Logic;
using QuizAppDB.Models;

var serviceCollection = new ServiceCollection();

// Configure services
serviceCollection.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite("Data Source=quizapp.db")); // Use SQLite database
serviceCollection.AddScoped<QuizRepository>(); // Register the QuizRepository
serviceCollection.AddScoped<QuizLogic>(); // Register the QuizLogic

var serviceProvider = serviceCollection.BuildServiceProvider();

// Create database if it doesn't exist
using (var scope = serviceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    dbContext.Database.EnsureCreated();
}

var quizLogic = serviceProvider.GetRequiredService<QuizLogic>();

while (true)
{
    Console.Clear();
    Console.WriteLine("\nMenu aplikacji Quiz:");
    Console.WriteLine("1. Stwórz quiz");
    Console.WriteLine("2. Rozwiąż quiz");
    Console.WriteLine("3. Usuń quiz");
    Console.WriteLine("4. Modyfikuj pytania");
    Console.WriteLine("ESC. Wyjdź");
    Console.Write("Wybierz opcję (naciśnij odpowiedni klawisz): ");

    var key = Console.ReadKey(intercept: true).Key;

    try
    {
        switch (key)
        {
            case ConsoleKey.D1:
                await quizLogic.CreateQuizAsync();
                break;
            case ConsoleKey.D2:
                await quizLogic.TakeQuizAsync();
                break;
            case ConsoleKey.D3:
                await quizLogic.DeleteQuizAsync();
                break;
            case ConsoleKey.D4:
                await quizLogic.ModifyQuestionsAsync();
                break;
            case ConsoleKey.Escape:
                return;
            default:
                Console.WriteLine("\nNieprawidłowy wybór. Spróbuj ponownie.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nBłąd: {ex.Message}");
    }

    Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
    Console.ReadKey();
}
