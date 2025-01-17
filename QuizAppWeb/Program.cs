using Microsoft.EntityFrameworkCore;
using QuizAppDB.Data;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Function to get the database path
string GetDatabasePath()
{
    // Start from the current directory and navigate up to the solution root
    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    DirectoryInfo? solutionRoot = FindSolutionRoot(currentDirectory);

    if (solutionRoot == null)
    {
        throw new InvalidOperationException("Unable to locate the solution root folder.");
    }

    // Construct the absolute path to the QuizAppDB.sqlite file
    string dbPath = Path.Combine(solutionRoot.FullName, "QuizAppDB", "bin", "Debug", "net8.0", "QuizAppDB.sqlite");

    // Verify the file exists
    if (!File.Exists(dbPath))
    {
        throw new FileNotFoundException("Database file not found at: " + dbPath);
    }

    return dbPath;
}

DirectoryInfo? FindSolutionRoot(string currentDirectory)
{
    DirectoryInfo? directory = new DirectoryInfo(currentDirectory);

    while (directory != null)
    {
        // Look for the solution file (.sln) in the current directory
        if (Directory.GetFiles(directory.FullName, "*.sln").Length > 0)
        {
            return directory;
        }

        // Move up to the parent directory
        directory = directory.Parent;
    }

    return null; // Solution root not found
}

// Dynamically resolve the database path
string databasePath = GetDatabasePath();

// Register DbContext with the dynamically resolved path
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

// Register repositories
builder.Services.AddScoped<QuizRepository>();

// Register controllers and views
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Quiz}/{action=Index}/{id?}");

app.Run();
