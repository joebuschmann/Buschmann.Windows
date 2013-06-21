﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Buschmann.Windows.TestApp.Converters
{
    internal class NotBoolConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool) value;
            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            return !boolValue;
        }
    }
}
