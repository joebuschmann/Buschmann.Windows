using System;
using System.Globalization;
using System.Windows.Data;

namespace Buschmann.Windows.TestApp.Converters
{
    internal class InterfacesValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = value as Type;

            if (type == null)
                return null;

            return type.GetInterfaces();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
