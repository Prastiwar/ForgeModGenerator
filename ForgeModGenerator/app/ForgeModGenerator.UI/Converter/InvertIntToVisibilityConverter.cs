using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ForgeModGenerator.Converter
{
    public class InvertIntToVisibilityConverter : IValueConverter
    {
        public Visibility PositiveValue { get; set; } = Visibility.Visible;
        public Visibility ZeroNegativeValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int num)
            {
                return num > 0 ? ZeroNegativeValue : PositiveValue;
            }
            return ZeroNegativeValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility? visibility = value as Visibility?;
            return visibility == PositiveValue ? 0 : 1;
        }
    }
}
