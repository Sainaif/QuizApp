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
                var questions = viewModel.Questions ?? Enumerable.Empty<QuestionViewModel>();
                int correctAnswers = questions.Count(q =>
                    q.Choices?.Where(c => c.IsCorrect).All(c => c.IsSelected) ?? false);

                int totalQuestions = questions.Count();
                double percentage = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

                string grade = percentage >= 90 ? "5 (bardzo dobry)"
                             : percentage >= 75 ? "4 (dobry)"
                             : percentage >= 50 ? "3 (dostateczny)"
                             : "2 (niedostateczny)";

                ResultsTextBlock.Text = $"Wynik:\nPoprawne odpowiedzi: {correctAnswers}/{totalQuestions}\n" +
                                        $"Procent: {percentage:F2}%\nOcena: {grade}";
                ResultsTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            QuizSelectionGrid.Visibility = Visibility.Visible;
            QuestionGrid.Visibility = Visibility.Collapsed;
            ResultsTextBlock.Visibility = Visibility.Collapsed;
        }

        private void QuitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
