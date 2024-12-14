using System.Text.Json;

class Program
{
    static string filePath = "quizzes.json"; // File path for saving/loading quizzes

    static void Main()
    {
        var quizManager = new QuizManager();

        while (true)
        {
            // Display the main menu for the user
            Console.WriteLine("\nMenu aplikacji Quiz:");
            Console.WriteLine("1. Stwórz quiz");
            Console.WriteLine("2. Rozwiąż quiz");
            Console.WriteLine("3. Usuń quiz");
            Console.WriteLine("4. Zapisz quizy do pliku");
            Console.WriteLine("5. Wczytaj quizy z pliku");
            Console.WriteLine("6. Wyjdź");
            Console.Write("Wybierz opcję: ");

            string? choice = Console.ReadLine();

            // Process user input and call the appropriate method
            switch (choice)
            {
                case "1":
                    CreateQuiz(quizManager);
                    break;
                case "2":
                    TakeQuiz(quizManager);
                    break;
                case "3":
                    DeleteQuiz(quizManager);
                    break;
                case "4":
                    quizManager.SaveQuizzesToFile(filePath);
                    Console.WriteLine("Quizy zapisane!");
                    break;
                case "5":
                    quizManager.LoadQuizzesFromFile(filePath);
                    Console.WriteLine("Quizy wczytane!");
                    break;
                case "6":
                    return; // Exit the application
                default:
                    Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
                    break;
            }
        }
    }

    static void CreateQuiz(QuizManager quizManager)
    {
        // Get quiz title from the user
        Console.Write("Podaj tytuł quizu: ");
        string? title = Console.ReadLine();
        if (string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Tytuł quizu nie może być pusty!");
            return;
        }

        Console.Write("Czy chcesz pokazać poprawne odpowiedzi po błędnych odpowiedziach? (tak/nie): ");
        string? showAnswersInput = Console.ReadLine();
        bool showCorrectAnswers = showAnswersInput?.ToLower() == "tak";

        var quiz = new Quiz { Title = title, ShowCorrectAnswers = showCorrectAnswers }; // Create a new quiz object

        while (true)
        {
            // Get question text from the user
            Console.Write("Podaj treść pytania (lub wpisz 'koniec', aby zakończyć): ");
            string? questionText = Console.ReadLine();
            if (string.IsNullOrEmpty(questionText) || questionText.ToLower() == "koniec")
                break;

            var question = new Question { Text = questionText }; // Create a new question object

            Console.Write("Podaj liczbę odpowiedzi dla tego pytania: ");
            if (int.TryParse(Console.ReadLine(), out int numChoices) && numChoices > 1)
            {
                // Get the answer choices from the user
                for (int i = 1; i <= numChoices; i++)
                {
                    Console.Write($"Podaj odpowiedź {i}: ");
                    string? choice = Console.ReadLine();
                    if (!string.IsNullOrEmpty(choice))
                        question.Choices.Add(choice);
                }

                // Get the number of correct answers from the user
                Console.Write("Podaj liczbę poprawnych odpowiedzi: ");
                if (int.TryParse(Console.ReadLine(), out int numCorrect) && numCorrect > 0 && numCorrect <= numChoices)
                {
                    Console.WriteLine("Podaj poprawne odpowiedzi według numerów (oddzielone przecinkami): ");
                    string? correctChoicesInput = Console.ReadLine();
                    var correctChoices = correctChoicesInput?.Split(',').Select(s => int.TryParse(s.Trim(), out int idx) ? idx - 1 : -1).Where(idx => idx >= 0 && idx < numChoices).ToList();

                    if (correctChoices != null && correctChoices.Count == numCorrect)
                    {
                        question.CorrectChoices = correctChoices; // Assign correct answers to the question
                        quiz.Questions.Add(question); // Add the question to the quiz
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowe poprawne odpowiedzi. Pytanie pominięte.");
                    }
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa liczba poprawnych odpowiedzi. Pytanie pominięte.");
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowa liczba odpowiedzi. Pytanie pominięte.");
            }
        }

        quizManager.AddQuiz(quiz); // Add the quiz to the quiz manager
        Console.WriteLine("Quiz utworzony!");
    }

    static void TakeQuiz(QuizManager quizManager)
    {
        if (!quizManager.Quizzes.Any())
        {
            Console.WriteLine("Brak dostępnych quizów.");
            return;
        }

        // Display available quizzes
        Console.WriteLine("Dostępne quizy:");
        for (int i = 0; i < quizManager.Quizzes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {quizManager.Quizzes[i].Title}");
        }

        // Let the user choose a quiz to take
        Console.Write("Wybierz numer quizu (lub wpisz 'wstecz', aby wrócić): ");
        string? input = Console.ReadLine();
        if (input?.ToLower() == "wstecz") return;

        if (int.TryParse(input, out int quizIndex) && quizIndex >= 1 && quizIndex <= quizManager.Quizzes.Count)
        {
            var quiz = quizManager.Quizzes[quizIndex - 1];
            int score = 0;

            // Iterate through the quiz questions
            foreach (var question in quiz.Questions)
            {
                Console.WriteLine($"\n{question.Text}");
                for (int i = 0; i < question.Choices.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {question.Choices[i]}");
                }

                Console.Write("Twoje odpowiedzi (oddzielone przecinkami): ");
                string? answersInput = Console.ReadLine();
                var answers = answersInput?.Split(',').Select(s => int.TryParse(s.Trim(), out int idx) ? idx - 1 : -1).Where(idx => idx >= 0 && idx < question.Choices.Count).ToList();

                // Check if the user's answers match the correct answers
                if (answers != null && answers.OrderBy(a => a).SequenceEqual(question.CorrectChoices.OrderBy(a => a)))
                {
                    score++;
                }
                else
                {
                    Console.WriteLine("Błędna odpowiedź.");
                    if (quiz.ShowCorrectAnswers)
                    {
                        Console.WriteLine("Poprawne odpowiedzi:");
                        foreach (var correct in question.CorrectChoices)
                        {
                            Console.WriteLine($"- {question.Choices[correct]}");
                        }
                    }
                }
            }

            Console.WriteLine($"\nTwój wynik: {score}/{quiz.Questions.Count}!");
        }
        else
        {
            Console.WriteLine("Nieprawidłowy numer quizu.");
        }
    }

    static void DeleteQuiz(QuizManager quizManager)
    {
        if (!quizManager.Quizzes.Any())
        {
            Console.WriteLine("Brak dostępnych quizów do usunięcia.");
            return;
        }

        // Display available quizzes
        Console.WriteLine("Dostępne quizy:");
        for (int i = 0; i < quizManager.Quizzes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {quizManager.Quizzes[i].Title}");
        }

        // Let the user choose a quiz to delete
        Console.Write("Wybierz numer quizu do usunięcia (lub wpisz 'wstecz', aby wrócić): ");
        string? input = Console.ReadLine();
        if (input?.ToLower() == "wstecz") return;

        if (int.TryParse(input, out int quizIndex) && quizIndex >= 1 && quizIndex <= quizManager.Quizzes.Count)
        {
            var deletedQuiz = quizManager.Quizzes[quizIndex - 1];
            quizManager.Quizzes.RemoveAt(quizIndex - 1); // Remove the selected quiz
            Console.WriteLine($"Quiz '{deletedQuiz.Title}' usunięty.");
        }
        else
        {
            Console.WriteLine("Nieprawidłowy numer quizu.");
        }
    }
}