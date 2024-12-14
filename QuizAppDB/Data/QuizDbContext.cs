using Microsoft.EntityFrameworkCore;
using QuizAppDB.Models; // Importing models for the quiz structure

namespace QuizAppDB.Data
{
    public class QuizDbContext : DbContext
    {
        // DbSet for quizzes, representing the Quiz table in the database
        public DbSet<Quiz> Quizzes { get; set; } = null!;
        // DbSet for questions, representing the Question table in the database
        public DbSet<Question> Questions { get; set; } = null!;
        // DbSet for choices, representing the Choice table in the database
        public DbSet<Choice> Choices { get; set; } = null!;

        // Constructor to initialize the DbContext with specific options
        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

        // Method to configure the model relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring one-to-many relationship between Quiz and Questions
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz) // A Question belongs to a Quiz
                .WithMany(quiz => quiz.Questions) // A Quiz has many Questions
                .HasForeignKey(q => q.QuizId) // Foreign key linking to the Quiz
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete behavior

            // Configuring one-to-many relationship between Question and Choices
            modelBuilder.Entity<Choice>()
                .HasOne(c => c.Question) // A Choice belongs to a Question
                .WithMany(q => q.Choices) // A Question has many Choices
                .HasForeignKey(c => c.QuestionId) // Foreign key linking to the Question
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete behavior
        }
    }
}