using ForgeModGenerator.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converters
{
    public class PathToModidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is string s ? McMod.GetModidFromPath(s) : null;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
