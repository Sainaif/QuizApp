using QuizAppDB.Models; // Importing models for quiz operations
using Microsoft.EntityFrameworkCore; // Importing Entity Framework Core for database access

namespace QuizAppDB.Data
{
    public class QuizRepository
    {
        private readonly QuizDbContext _context; // DbContext instance for accessing the database

        // Constructor to inject the DbContext dependency
        public QuizRepository(QuizDbContext context)
        {
            _context = context;
        }

        // Retrieves all quizzes with their associated questions and choices
        public async Task<List<Quiz>> GetAllQuizzesAsync()
        {
            return await _context.Quizzes
                .Include(q => q.Questions) // Include related Questions
                .ThenInclude(q => q.Choices) // Include Choices for each Question
                .ToListAsync(); // Execute and return the results as a list
        }

        // Retrieves a specific quiz by ID, including its questions and choices
        public async Task<Quiz?> GetQuizByIdAsync(int id)
        {
            return await _context.Quizzes
                .Include(q => q.Questions) // Include related Questions
                .ThenInclude(q => q.Choices) // Include Choices for each Question
                .FirstOrDefaultAsync(q => q.Id == id); // Return the quiz with the specified ID
        }

        // Adds a new quiz to the database
        public async Task AddQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz); // Add the quiz to the DbSet
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        // Updates an existing quiz in the database
        public async Task UpdateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Update(quiz); // Mark the quiz entity as modified
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        // Deletes a quiz and its related data from the database
        public async Task DeleteQuizAsync(int id)
        {
            var quiz = await GetQuizByIdAsync(id); // Retrieve the quiz by ID
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz); // Remove the quiz entity
                await _context.SaveChangesAsync(); // Save changes to the database
            }
        }
    }
}