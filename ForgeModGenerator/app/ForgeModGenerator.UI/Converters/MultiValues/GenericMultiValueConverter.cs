using System;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converters
{
    public abstract class GenericMultiValueConverter<TTarget> : GenericMultiValueConverter<TTarget, object> { }

    public abstract class GenericMultiValueConverter<TTarget, TParameter> : IMultiValueConverter
    {
        protected abstract TTarget Convert(object[] values, Type targetType, TParameter parameter, CultureInfo culture);
        protected abstract object[] ConvertBack(TTarget value, Type[] targetTypes, TParameter parameter, CultureInfo culture);

        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && !(parameter is TParameter))
            {
                throw new InvalidCastException(string.Format("In order to use the generic IValueConverter you have to use the correct type as ConvertParameter. The passing type was {0} but the expected is {1}", parameter.GetType(), typeof(TParameter)));
            }
            return Convert(values, targetType, (TParameter)parameter, culture);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (!(value is TTarget))
            {
                throw new InvalidCastException(string.Format("In order to use the generic IValueConverter you have to use the correct type. The passing type was {0} but the expected is {1}", value.GetType(), typeof(TTarget)));
            }
            if (parameter != null && !(parameter is TParameter))
            {
                throw new InvalidCastException(string.Format("In order to use the generic IValueConverter you have to use the correct type as ConvertParameter. The passing type was {0} but the expected is {1}", parameter.GetType(), typeof(TParameter)));
            }
            return ConvertBack((TTarget)value, targetTypes, (TParameter)parameter, culture);
        }
    }
}
