using ForgeModGenerator.Utility;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;

namespace ForgeModGenerator.Converters
{
    public class GetCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string methodName = null;
            object[] passParameters = null;
            if (parameter is string parameterString)
            {
                methodName = parameterString;
            }
            else if (parameter is object[] parameters)
            {
                if (parameters[0] is string firstParameter)
                {
                    methodName = firstParameter;
                    passParameters = new object[parameters.Length - 1];
                    Array.Copy(parameters, 1, passParameters, 0, passParameters.Length); // skip methodName
                }
                else if (parameters[0] is BindingExpression bindExpressionParameter)
                {
                    methodName = bindExpressionParameter.GetResolvedValue<string>();
                }
                else if (parameters[0] is Binding bindParameter)
                {
                    methodName = bindParameter.GetResolvedValue<string>();
                }
            }
            if (value == null || string.IsNullOrEmpty(methodName))
            {
                return null;
            }
            MethodInfo methodInfo = GetMethodInfo(value, methodName);
            return methodInfo != null ? GetCommand(methodInfo, value, passParameters) : null;
        }

        private MethodInfo GetMethodInfo(object instance, string methodName)
        {
            MethodInfo methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo == null)
            {
                methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo == null)
                {
                    methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.Static);
                }
            }
            return methodInfo;
        }

        private ICommand GetCommand(MethodInfo method, object instance, object[] parameters)
        {

            ParameterInfo[] parameterInfos = method.GetParameters();
            if (parameterInfos.Length > 0)
            {
                if (parameters is null)
                {
                    throw new ArgumentNullException(nameof(parameters));
                }
                if (parameters.Length != parameterInfos.Length)
                {
                    throw new ArgumentException($"Length of parameters needed for method {method.Name} doesn't match with passed parameters");
                }
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type wantedType = parameterInfos[i].ParameterType;
                    if (parameters[i] != null)
                    {
                        Type passedType = parameters[i].GetType();
                        if (typeof(BindingExpression).IsAssignableFrom(passedType))
                        {
                            object value = ((BindingExpression)parameters[i]).GetResolvedValue();
                            passedType = value?.GetType();
                            parameters[i] = value;
                        }
                        else if (typeof(Binding).IsAssignableFrom(passedType))
                        {
                            object value = ((Binding)parameters[i]).GetResolvedValue();
                            passedType = value?.GetType();
                            parameters[i] = value;
                        }
                        if (!wantedType.IsAssignableFrom(passedType))
                        {
                            throw new ArgumentException($"Can't pass parameter type of {passedType} at index {i} to type {wantedType}");
                        }
                    }
                }
            }
            else
            {
                parameters = null;
            }
            return method.Invoke(instance, parameters) as ICommand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException($"{nameof(ConvertBack)} is not supported");
    }

}