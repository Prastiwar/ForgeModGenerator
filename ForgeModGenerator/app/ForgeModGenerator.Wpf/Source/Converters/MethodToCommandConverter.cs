using Prism.Commands;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;

namespace ForgeModGenerator.Converters
{
    public class MethodToCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string methodName = parameter as string;
            if (value == null || string.IsNullOrEmpty(methodName))
            {
                return null;
            }

            MethodInfo methodInfo = value.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo == null)
            {
                methodInfo = value.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo == null)
                {
                    methodInfo = value.GetType().GetMethod(methodName, BindingFlags.Static);
                    return methodInfo != null ? CreateCommand(methodInfo, null) : null;
                }
            }
            return CreateCommand(methodInfo, value);
        }

        private ICommand CreateCommand(MethodInfo method, object instance)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                return new DelegateCommand(() => { method.Invoke(instance, null); });
            }
            else if (parameters.Length == 1)
            {
                Type paramType = parameters[0].ParameterType;
                Type[] paramTypeArray = new Type[] { paramType };
                Type commandType = typeof(DelegateCommand<>).MakeGenericType(paramTypeArray);
                Action<object> action = (obj) => { method.Invoke(instance, new object[] { obj }); };
                return (ICommand)Activator.CreateInstance(commandType, action);
            }
            else
            {
                throw new NotSupportedException($"Cannot make {nameof(ICommand)} for more than 1 parameter");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException($"{nameof(ConvertBack)} is not supported");
    }

}