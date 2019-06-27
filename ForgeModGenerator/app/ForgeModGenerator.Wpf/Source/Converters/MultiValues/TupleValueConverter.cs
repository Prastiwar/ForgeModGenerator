using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ForgeModGenerator.Converters
{
    public class GenericTupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(x => x == null || x == DependencyProperty.UnsetValue))
            {
                return null; // we cannot make generic type from null object
            }
            int length = values.Length;
            Type[] types = new Type[length];
            for (int i = 0; i < length; i++)
            {
                types[i] = values[i].GetType();
            }
            MethodInfo methodInfo = typeof(Tuple).GetMethods().First(x => x.IsGenericMethod && x.GetGenericArguments().Length == length);
            methodInfo = methodInfo.MakeGenericMethod(types);
            return methodInfo.Invoke(null, values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException(nameof(GenericTupleConverter) + " works only one way");
    }

    public class TupleValueConverter<T1, T2> : GenericMultiValueConverter<Tuple<T1, T2>, object>
    {
        protected override Tuple<T1, T2> Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            T1 item1 = values[0] is T1 val1 ? val1 : default;
            T2 item2 = values[1] is T2 val2 ? val2 : default;
            return new Tuple<T1, T2>(item1, item2);
        }
        protected override object[] ConvertBack(Tuple<T1, T2> value, Type[] targetTypes, object parameter, CultureInfo culture) => new object[] { value.Item1, value.Item2 };
    }

    public class TupleValueConverter<T1, T2, T3> : GenericMultiValueConverter<Tuple<T1, T2, T3>, object>
    {
        protected override Tuple<T1, T2, T3> Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            T1 item1 = values[0] is T1 val1 ? val1 : default;
            T2 item2 = values[1] is T2 val2 ? val2 : default;
            T3 item3 = values[2] is T3 val3 ? val3 : default;
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }
        protected override object[] ConvertBack(Tuple<T1, T2, T3> value, Type[] targetTypes, object parameter, CultureInfo culture) => new object[] { value.Item1, value.Item2, value.Item3 };
    }

    public class StringListStringConverter : TupleValueConverter<ObservableCollection<string>, string> { }
}
