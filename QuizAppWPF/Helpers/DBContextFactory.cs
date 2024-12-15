using Microsoft.EntityFrameworkCore;
using QuizAppDB.Data;

namespace QuizAppWPF.Helpers
{
    public static class DBContextFactory
    {
        public static QuizDbContext Create()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseSqlite($"Data Source={DBHelper.GetDatabasePath()}")
                .Options;

            return new QuizDbContext(options);
        }
    }
}
