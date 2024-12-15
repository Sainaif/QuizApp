using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using QuizAppDB.Data;
using QuizAppDB.Models;
using QuizAppWPF.Helpers;

namespace QuizAppWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly QuizRepository _quizRepository;

        public ObservableCollection<Quiz>? Quizzes { get; set; } = new();
        public ObservableCollection<QuestionViewModel>? Questions { get; set; } = new();
        public bool ShowResults { get; set; } = false; // Controls result display

        private Quiz? _selectedQuiz;
        public Quiz? SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged();
                LoadQuestions();
                ShowResults = false; // Reset results when a new quiz is selected
                OnPropertyChanged(nameof(ShowResults));
            }
        }

        public MainViewModel()
        {
            var dbContext = DBContextFactory.Create();
            _quizRepository = new QuizRepository(dbContext);

            _ = LoadQuizzesAsync();
        }

        private async Task LoadQuizzesAsync()
        {
            var quizzesFromDb = await _quizRepository.GetAllQuizzesAsync();
            if (quizzesFromDb != null)
            {
                foreach (var quiz in quizzesFromDb)
                {
                    Quizzes?.Add(quiz);
                }
            }
        }

        private void LoadQuestions()
        {
            Questions?.Clear();
            if (SelectedQuiz?.Questions != null)
            {
                foreach (var question in SelectedQuiz.Questions)
                {
                    Questions?.Add(new QuestionViewModel(question));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
