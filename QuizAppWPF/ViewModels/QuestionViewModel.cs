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
