using System;
using System.Globalization;
using System.Windows.Data;

namespace Buschmann.Windows.Controls.Converters
{
    internal class CommandStringDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().Length == 0)
                return "[default]";

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
