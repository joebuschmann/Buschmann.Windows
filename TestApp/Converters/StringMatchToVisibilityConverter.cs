using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Buschmann.Windows.TestApp.Converters
{
    internal class StringMatchToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            string parameterValue = parameter as string;

            if (stringValue == parameterValue)
                return Visibility.Visible;
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
