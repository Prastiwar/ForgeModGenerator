using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converters
{
    internal class DataBindingDebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}
