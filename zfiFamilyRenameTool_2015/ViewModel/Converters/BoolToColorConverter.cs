namespace zfiFamilyRenameTool.ViewModel.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && !b)
                return (SolidColorBrush)Application.Current.FindResource("ErrorBrush");
            return (SolidColorBrush)Application.Current.FindResource("BlackBrush");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}