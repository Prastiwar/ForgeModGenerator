using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ForgeModGenerator.Converter
{
    public class TupleValueConverter<T1, T2> : GenericMultiValueConverter<Tuple<T1, T2>, object>
    {
        protected override Tuple<T1, T2> Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            T1 item1 = values[0] is T1 val1 ? val1 : default(T1);
            T2 item2 = values[1] is T2 val2 ? val2 : default(T2);
            return new Tuple<T1, T2>(item1, item2);
        }
        protected override object[] ConvertBack(Tuple<T1, T2> value, Type[] targetTypes, object parameter, CultureInfo culture) => new object[] { value.Item1, value.Item2 };
    }

    public class TupleValueConverter<T1, T2, T3> : GenericMultiValueConverter<Tuple<T1, T2, T3>, object>
    {
        protected override Tuple<T1, T2, T3> Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            T1 item1 = values[0] is T1 val1 ? val1 : default(T1);
            T2 item2 = values[1] is T2 val2 ? val2 : default(T2);
            T3 item3 = values[2] is T3 val3 ? val3 : default(T3);
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }
        protected override object[] ConvertBack(Tuple<T1, T2, T3> value, Type[] targetTypes, object parameter, CultureInfo culture) => new object[] { value.Item1, value.Item2, value.Item3 };
    }

    public class StringListStringConverter : TupleValueConverter<ObservableCollection<string>, string> { }
}
