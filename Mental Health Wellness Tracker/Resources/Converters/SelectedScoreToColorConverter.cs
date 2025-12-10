using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace Mental_Health_Wellness_Tracker.Resources.Converters
{
    public class SelectedScoreToColorConverter : IValueConverter
    {
        private Color SelectedColor { get; } = Color.FromHex("#00CED1"); // HighlightTeal
        private Color DefaultColor { get; } = Color.FromHex("#008080"); // PrimaryBlue

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedScore && parameter is string parameterString)
            {
                if (int.TryParse(parameterString, out int parameterScore))
                {
                    // If the selected score matches the parameter score (the button's value), highlight it.
                    return selectedScore == parameterScore ? SelectedColor : DefaultColor;
                }
            }
            // If the selected score is null (initial state), use the default color.
            return DefaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}