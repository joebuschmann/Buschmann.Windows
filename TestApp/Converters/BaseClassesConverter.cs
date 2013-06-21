using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Buschmann.Windows.TestApp.Converters
{
    internal class BaseClassesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = value as Type;

            if (type == null)
                return null;

            return GetBaseClasses(type);
        }

        private IEnumerable<Type> GetBaseClasses(Type subClass)
        {
            Type baseClass = subClass.BaseType;

            while (baseClass != null)
            {
                yield return baseClass;
                baseClass = baseClass.BaseType;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
