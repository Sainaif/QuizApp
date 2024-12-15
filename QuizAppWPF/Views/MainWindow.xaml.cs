using System.Linq;
using System.Windows;
using QuizAppWPF.ViewModels;

namespace QuizAppWPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel && viewModel.SelectedQuiz != null)
            {
                QuizSelectionGrid.Visibility = Visibility.Collapsed;
                QuestionGrid.Visibility = Visibility.Visible;

                // Reset answers and interaction state for all questions
                foreach (var question in viewModel.Questions)
                {
                    question.ShowAnswers = false;

                    foreach (var choice in question.Choices)
                    {
                        choice.IsSelected = false;
                    }
                }

                ResultsTextBlock.Text = string.Empty;
                ResultsTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Wybierz quiz, aby rozpocząć.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SubmitAnswers_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // Count only questions where all correct choices are selected and no incorrect ones
                int correctAnswers = viewModel.Questions.Count(question =>
                    question.Choices.Where(choice => choice.IsCorrect).All(choice => choice.IsSelected) &&
                    question.Choices.Where(choice => !choice.IsCorrect).All(choice => !choice.IsSelected));

                int totalQuestions = viewModel.Questions.Count;
                double percentage = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

                string grade = percentage >= 90 ? "5 (bardzo dobry)"
                             : percentage >= 75 ? "4 (dobry)"
                             : percentage >= 50 ? "3 (dostateczny)"
                             : "2 (niedostateczny)";

                ResultsTextBlock.Text = $"Wynik:\nPoprawne odpowiedzi: {correctAnswers}/{totalQuestions}\n" +
                                        $"Procent: {percentage:F2}%\nOcena: {grade}";
                ResultsTextBlock.Visibility = Visibility.Visible;

                // Show correct/incorrect answers
                foreach (var question in viewModel.Questions)
                {
                    question.ShowAnswers = true;
                }
            }
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // Fully reset the quiz state
                foreach (var question in viewModel.Questions)
                {
                    question.ShowAnswers = false; // Hide correct/incorrect answers

                    foreach (var choice in question.Choices)
                    {
                        // Uncheck all chosen answers
                        choice.IsSelected = false;
                    }
                }

                // Clear results text
                ResultsTextBlock.Text = string.Empty;
                ResultsTextBlock.Visibility = Visibility.Collapsed;

                // Reset SelectedQuiz
                viewModel.SelectedQuiz = null;
                viewModel.Questions.Clear(); // Clear any loaded questions
            }

            // Reset visibility to show quiz selection
            QuizSelectionGrid.Visibility = Visibility.Visible;
            QuestionGrid.Visibility = Visibility.Collapsed;
        }

        private void QuitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
