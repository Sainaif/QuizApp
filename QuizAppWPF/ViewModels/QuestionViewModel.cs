using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using QuizAppDB.Models;

namespace QuizAppWPF.ViewModels
{
    public class QuestionViewModel : INotifyPropertyChanged
    {
        public string QuestionText { get; }
        public ObservableCollection<ChoiceViewModel> Choices { get; }

        private bool _showAnswers;
        public bool ShowAnswers
        {
            get => _showAnswers;
            set
            {
                _showAnswers = value;
                OnPropertyChanged();
                foreach (var choice in Choices)
                {
                    choice.ShowAnswers = value;
                }
            }
        }

        public QuestionViewModel(Question question)
        {
            QuestionText = question.Text ?? string.Empty;
            Choices = new ObservableCollection<ChoiceViewModel>(
                question.Choices?.Select(c => new ChoiceViewModel(c)) ?? Enumerable.Empty<ChoiceViewModel>());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ChoiceViewModel : INotifyPropertyChanged
    {
        public string Text { get; }
        public bool IsCorrect { get; }
        public bool IsSelected { get; set; }

        private bool _showAnswers;
        public bool ShowAnswers
        {
            get => _showAnswers;
            set
            {
                _showAnswers = value;
                OnPropertyChanged();
            }
        }

        public ChoiceViewModel(Choice choice)
        {
            Text = choice.Text ?? string.Empty;
            IsCorrect = choice.IsCorrect;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
