using Prism.Commands;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            Type[] tupleArgumentTypes = GetTupleTypes(parameter);
            if (tupleArgumentTypes.Length != values.Length)
            {
                return null; // values doesn't match tuple arguments we want to create
            }
            MethodInfo methodInfo = GetCreateTupleMethod(values, tupleArgumentTypes);
            return methodInfo.Invoke(null, values);
        }

        private MethodInfo GetCreateTupleMethod(object[] values, Type[] tupleArgumentTypes)
        {
            MethodInfo methodInfo = typeof(Tuple).GetMethods().First(x => x.IsGenericMethod && x.GetGenericArguments().Length == values.Length);
            return methodInfo.MakeGenericMethod(tupleArgumentTypes);
        }

        private Type[] GetTupleTypes(object parameter)
        {
            Type[] genericTypes = parameter.GetType().GetGenericArguments();
            bool isParameterGenericDelegateCommand = parameter != null &&
                                                     parameter.GetType().IsGenericType &&
                                                     genericTypes.Length == 1 &&
                                                     typeof(DelegateCommandBase).IsAssignableFrom(parameter.GetType());
            if (isParameterGenericDelegateCommand)
            {
                Type argumentType = genericTypes[0];
                if (typeof(ITuple).IsAssignableFrom(argumentType))
                {
                    return argumentType.GetGenericArguments();
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException(nameof(GenericTupleConverter) + " works only one way");

    }
}
