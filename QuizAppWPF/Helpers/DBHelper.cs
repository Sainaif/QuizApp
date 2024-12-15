using System;
using System.IO;

namespace QuizAppWPF.Helpers
{
    public static class DBHelper
    {
        public static string GetDatabasePath()
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

        private static DirectoryInfo? FindSolutionRoot(string currentDirectory)
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
    }
}
