using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Mental_Health_Wellness_Tracker.Models
{
    // Question acts as an Item ViewModel
    public class Question : INotifyPropertyChanged
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Answers { get; set; } = new List<string>();

        public int SelectedScore { get; set; } = -1; // -1 means no answer selected

        private Color _c0 = Colors.DeepSkyBlue;
        private Color _c1 = Colors.DeepSkyBlue;
        private Color _c2 = Colors.DeepSkyBlue;
        private Color _c3 = Colors.DeepSkyBlue;

        public Color Color0 { get => _c0; set { _c0 = value; OnPropertyChanged(); } }
        public Color Color1 { get => _c1; set { _c1 = value; OnPropertyChanged(); } }
        public Color Color2 { get => _c2; set { _c2 = value; OnPropertyChanged(); } }
        public Color Color3 { get => _c3; set { _c3 = value; OnPropertyChanged(); } }

        public ICommand AnswerTappedCommand { get; }

        public Question()
        {
            AnswerTappedCommand = new ViewModels.RelayCommand(OnAnswerTapped);
        }

        private void OnAnswerTapped(object parameter)
        {
            if (parameter is string indexString && int.TryParse(indexString, out int index))
            {
                SelectedScore = index;

                // Reset all colors to default
                Color0 = Colors.DeepSkyBlue;
                Color1 = Colors.DeepSkyBlue;
                Color2 = Colors.DeepSkyBlue;
                Color3 = Colors.DeepSkyBlue;

                // Highlight the selected answer, mirroring the original logic
                Color selectedColor = Colors.OrangeRed;
                if (index == 0) Color0 = selectedColor;
                else if (index == 1) Color1 = selectedColor;
                else if (index == 2) Color2 = selectedColor;
                else if (index == 3) Color3 = selectedColor;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // --- BLUEPRINT FOR A SINGLE HISTORY RECORD (MOVED FROM AssessmentState.cs) ---
    public class AssessmentHistoryItem
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public string Status { get; set; }
        public string TestType { get; set; } = "Wellness Assessment";

        public List<int> AnswerData { get; set; } = new List<int>();
    }
}