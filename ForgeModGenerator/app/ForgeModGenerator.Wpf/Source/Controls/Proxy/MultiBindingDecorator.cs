using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ForgeModGenerator.Controls
{
    /// <summary> MultiBinding that allows to bind ConverterParameter </summary>
    public class MultiBindingDecorator : MarkupExtension
    {
        public MultiBinding MultiBinding { get; set; }
        public Binding ConverterParameter { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            MultiBinding.Bindings.Add(ConverterParameter);
            MultiValueConverterAdapter adapter = new MultiValueConverterAdapter {
                Converter = MultiBinding.Converter
            };
            MultiBinding.Converter = adapter;
            return MultiBinding.ProvideValue(serviceProvider);
        }

        /// <summary> Converter adapter that takes last binding as ConverterParameter </summary>
        private class MultiValueConverterAdapter : IMultiValueConverter
        {
            public IMultiValueConverter Converter { get; set; }

            private object lastParameter;

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (Converter == null)
                {
                    return null;
                }
                lastParameter = values[values.Length - 1];
                object[] nextValues = new object[values.Length - 1];
                Array.Copy(values, 0, nextValues, 0, nextValues.Length);
                return Converter.Convert(nextValues, targetType, lastParameter, culture);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (Converter == null)
                {
                    return null;
                }
                Type[] nextTypes = new Type[targetTypes.Length - 1];
                Array.Copy(targetTypes, 0, nextTypes, 0, nextTypes.Length);
                return Converter.ConvertBack(value, nextTypes, lastParameter, culture);
            }
        }
    }
}
