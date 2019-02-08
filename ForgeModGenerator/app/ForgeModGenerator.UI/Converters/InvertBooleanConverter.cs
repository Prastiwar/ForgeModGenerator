using System;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converters
{
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is bool origin ? !origin : false;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is bool origin ? !origin : false;
    }
}
