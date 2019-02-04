using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ForgeModGenerator.Converter
{
    public class BindMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
            values[0] is IMultiValueConverter multConverter ? multConverter.Convert(values.Skip(1).ToArray(), targetType, parameter, culture) : null;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
