using System;
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
}
