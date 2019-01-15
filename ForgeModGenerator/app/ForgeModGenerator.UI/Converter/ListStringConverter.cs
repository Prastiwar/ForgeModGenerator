using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converter
{
    public class ListStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => new Tuple<ObservableCollection<string>, string>(values[0] as ObservableCollection<string>, values[1] as string);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Tuple<ObservableCollection<string>, string> tuple = value as Tuple<ObservableCollection<string>, string>;
            return new object[] { tuple.Item1, tuple.Item2 };
        }
    }
}
