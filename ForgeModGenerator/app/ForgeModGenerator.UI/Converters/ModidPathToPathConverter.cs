using System;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converters
{
    public class ModidPathToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is string s ? s.Remove(0, s.IndexOf(':') + 1) : null;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
