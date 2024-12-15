using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using QuizAppDB.Models;

namespace QuizAppWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Collection of questions for the selected quiz
        private ObservableCollection<QuestionViewModel> _questions = new ObservableCollection<QuestionViewModel>();
        public ObservableCollection<QuestionViewModel> Questions
        {
            get => _questions;
            set
            {
                _questions = value;
                OnPropertyChanged();
            }
        }

        // The currently selected quiz
        private Quiz? _selectedQuiz;
        public Quiz? SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                LoadQuestions();
                OnPropertyChanged();
            }
        }

        // Collection of all available quizzes
        public ObservableCollection<Quiz> Quizzes { get; } = new ObservableCollection<Quiz>();

        // Constructor to initialize the ViewModel
        public MainViewModel()
        {
            LoadQuizzes();
        }

        // Loads all quizzes from the database
        private void LoadQuizzes()
        {
            using var context = Helpers.DBContextFactory.Create();
            var quizzes = context.Quizzes.ToList();
            Quizzes.Clear();
            quizzes.ForEach(q => Quizzes.Add(q));
        }

        // Loads questions for the selected quiz from the database
        private void LoadQuestions()
        {
            Questions.Clear();
            if (SelectedQuiz != null)
            {
                using var context = Helpers.DBContextFactory.Create();
                var questions = context.Questions
                    .Where(q => q.QuizId == SelectedQuiz.Id)
                    .ToList();

                foreach (var question in questions)
                {
                    // Load choices for each question
                    var choices = context.Choices
                        .Where(c => c.QuestionId == question.Id)
                        .ToList();

                    Questions.Add(new QuestionViewModel(question, choices, SelectedQuiz.ShowCorrectAnswers));
                }
            }
        }

        // Event to notify when a property value changes
        public event PropertyChangedEventHandler? PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
