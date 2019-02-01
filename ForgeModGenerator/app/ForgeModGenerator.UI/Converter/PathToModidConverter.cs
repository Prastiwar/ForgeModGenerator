using ForgeModGenerator.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converter
{
    public class PathToModidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is string s ? Mod.GetModidFromPath(s) : null;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
