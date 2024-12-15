using Microsoft.EntityFrameworkCore;
using QuizAppDB.Data;

namespace QuizAppWPF.Helpers
{
    public static class DBContextFactory
    {
        /// <summary>
        /// Creates a new instance of QuizDbContext with the specified options.
        /// </summary>
        /// <returns>A new instance of QuizDbContext.</returns>
        public static QuizDbContext Create()
        {
            // Build the options for the DbContext, using SQLite as the database provider
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseSqlite($"Data Source={DBHelper.GetDatabasePath()}")
                .Options;

            // Return a new instance of QuizDbContext with the specified options
            return new QuizDbContext(options);
        }
    }
}
