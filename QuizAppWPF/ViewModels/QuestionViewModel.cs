using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using QuizAppDB.Models;

namespace QuizAppWPF.ViewModels
{
    public class QuestionViewModel : INotifyPropertyChanged
    {
        // The text of the question
        public string QuestionText { get; }

        // Collection of choices for the question
        public ObservableCollection<ChoiceViewModel> Choices { get; }

        private bool _showAnswers;
        private readonly bool _canShowCorrectAnswers;

        // Indicates whether to show the answers
        public bool ShowAnswers
        {
            get => _showAnswers;
            set
            {
                _showAnswers = value && _canShowCorrectAnswers; // Show only if allowed
                OnPropertyChanged();
                foreach (var choice in Choices)
                {
                    choice.ShowAnswers = _showAnswers;
                }
            }
        }

        // Constructor to initialize the QuestionViewModel
        public QuestionViewModel(Question question, List<Choice> choices, bool canShowCorrectAnswers)
        {
            QuestionText = question.Text ?? string.Empty;
            _canShowCorrectAnswers = canShowCorrectAnswers;
            Choices = new ObservableCollection<ChoiceViewModel>(
                choices.Select(c => new ChoiceViewModel(c)));
        }

        // Event to notify when a property changes
        public event PropertyChangedEventHandler? PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ChoiceViewModel : INotifyPropertyChanged
    {
        // The text of the choice
        public string Text { get; }

        // Indicates whether the choice is correct
        public bool IsCorrect { get; }

        // Indicates whether the choice is selected
        public bool IsSelected { get; set; }

        private bool _showAnswers;

        // Indicates whether to show the answers
        public bool ShowAnswers
        {
            get => _showAnswers;
            set
            {
                _showAnswers = value;
                OnPropertyChanged();
            }
        }

        // Constructor to initialize the ChoiceViewModel
        public ChoiceViewModel(Choice choice)
        {
            Text = choice.Text ?? string.Empty;
            IsCorrect = choice.IsCorrect;
        }

        // Event to notify when a property changes
        public event PropertyChangedEventHandler? PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
