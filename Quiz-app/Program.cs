using System.Text.Json;

class Program
{
    static string filePath = "quizzes.json"; // File path for saving/loading quizzes

    static void Main()
    {
        var quizManager = new QuizManager();

        while (true)
        {
            Console.WriteLine("\nQuiz App Menu:");
            Console.WriteLine("1. Create a Quiz");
            Console.WriteLine("2. Take a Quiz");
            Console.WriteLine("3. Delete a Quiz");
            Console.WriteLine("4. Save Quizzes to File");
            Console.WriteLine("5. Load Quizzes from File");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

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
                    Console.WriteLine("Quizzes saved!");
                    break;
                case "5":
                    quizManager.LoadQuizzesFromFile(filePath);
                    Console.WriteLine("Quizzes loaded!");
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    static void CreateQuiz(QuizManager quizManager)
    {
        Console.Write("Enter quiz title: ");
        string? title = Console.ReadLine();
        if (string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Quiz title cannot be empty!");
            return;
        }

        var quiz = new Quiz { Title = title };

        while (true)
        {
            Console.Write("Enter question text (or type 'done' to finish): ");
            string? questionText = Console.ReadLine();
            if (string.IsNullOrEmpty(questionText) || questionText.ToLower() == "done")
                break;

            var question = new Question { Text = questionText };

            Console.Write("Enter the number of answer choices for this question: ");
            if (int.TryParse(Console.ReadLine(), out int numChoices) && numChoices > 1)
            {
                for (int i = 1; i <= numChoices; i++)
                {
                    Console.Write($"Enter choice {i}: ");
                    string? choice = Console.ReadLine();
                    if (!string.IsNullOrEmpty(choice))
                        question.Choices.Add(choice);
                }

                Console.Write("Enter the number of correct answers for this question: ");
                if (int.TryParse(Console.ReadLine(), out int numCorrect) && numCorrect > 0 && numCorrect <= numChoices)
                {
                    Console.WriteLine("Enter the correct choices by their numbers (separated by commas): ");
                    string? correctChoicesInput = Console.ReadLine();
                    var correctChoices = correctChoicesInput?.Split(',').Select(s => int.TryParse(s.Trim(), out int idx) ? idx - 1 : -1).Where(idx => idx >= 0 && idx < numChoices).ToList();

                    if (correctChoices != null && correctChoices.Count == numCorrect)
                    {
                        question.CorrectChoices = correctChoices;
                        quiz.Questions.Add(question);
                    }
                    else
                    {
                        Console.WriteLine("Invalid correct choices. Skipping question.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid number of correct answers. Skipping question.");
                }
            }
            else
            {
                Console.WriteLine("Invalid number of choices. Skipping question.");
            }
        }

        quizManager.AddQuiz(quiz);
        Console.WriteLine("Quiz created!");
    }

    static void TakeQuiz(QuizManager quizManager)
    {
        if (!quizManager.Quizzes.Any())
        {
            Console.WriteLine("No quizzes available.");
            return;
        }

        Console.WriteLine("Available quizzes:");
        for (int i = 0; i < quizManager.Quizzes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {quizManager.Quizzes[i].Title}");
        }

        Console.Write("Choose a quiz number to take (or type 'back' to return): ");
        string? input = Console.ReadLine();
        if (input?.ToLower() == "back") return;

        if (int.TryParse(input, out int quizIndex) && quizIndex >= 1 && quizIndex <= quizManager.Quizzes.Count)
        {
            var quiz = quizManager.Quizzes[quizIndex - 1];
            int score = 0;

            foreach (var question in quiz.Questions)
            {
                Console.WriteLine($"\n{question.Text}");
                for (int i = 0; i < question.Choices.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {question.Choices[i]}");
                }

                Console.Write("Your answers (separated by commas): ");
                string? answersInput = Console.ReadLine();
                var answers = answersInput?.Split(',').Select(s => int.TryParse(s.Trim(), out int idx) ? idx - 1 : -1).Where(idx => idx >= 0 && idx < question.Choices.Count).ToList();

                if (answers != null && answers.OrderBy(a => a).SequenceEqual(question.CorrectChoices.OrderBy(a => a)))
                {
                    score++;
                }
            }

            Console.WriteLine($"\nYou scored {score}/{quiz.Questions.Count}!");
        }
        else
        {
            Console.WriteLine("Invalid quiz number.");
        }
    }

    static void DeleteQuiz(QuizManager quizManager)
    {
        if (!quizManager.Quizzes.Any())
        {
            Console.WriteLine("No quizzes available to delete.");
            return;
        }

        Console.WriteLine("Available quizzes:");
        for (int i = 0; i < quizManager.Quizzes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {quizManager.Quizzes[i].Title}");
        }

        Console.Write("Choose a quiz number to delete (or type 'back' to return): ");
        string? input = Console.ReadLine();
        if (input?.ToLower() == "back") return;

        if (int.TryParse(input, out int quizIndex) && quizIndex >= 1 && quizIndex <= quizManager.Quizzes.Count)
        {
            var deletedQuiz = quizManager.Quizzes[quizIndex - 1];
            quizManager.Quizzes.RemoveAt(quizIndex - 1);
            Console.WriteLine($"Quiz '{deletedQuiz.Title}' deleted.");
        }
        else
        {
            Console.WriteLine("Invalid quiz number.");
        }
    }
}