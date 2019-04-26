using ForgeModGenerator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ForgeModGenerator.Converters
{
    public class BoolValueConverter<T> : IValueConverter where T : IComparable
    {
        public T TrueValue { get; set; }
        public T FalseValue { get; set; }
        public bool Invert { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ConvertoToGeneric(value);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ConvertToBool(value);

        protected T ConvertoToGeneric(object value) => !Invert
                                                    ? (value is bool ? (bool)value ? TrueValue : FalseValue : FalseValue)
                                                    : (value is bool ? (bool)value ? FalseValue : TrueValue : TrueValue);

        protected bool ConvertToBool(object value) => !Invert
                                                 ? (value is T val1 ? EqualityComparer<T>.Default.Equals(val1, TrueValue) ? true : false : false)
                                                 : (value is T val2 ? EqualityComparer<T>.Default.Equals(val2, TrueValue) ? false : true : true);

    }

    public class BoolValueInvertedConverter<T> : BoolValueConverter<T>, IValueConverter where T : IComparable
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ConvertToBool(value);
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ConvertoToGeneric(value);
    }

    public class BoolToScrollVisibilityConverter : BoolValueConverter<ScrollBarVisibility>
    {
        public BoolToScrollVisibilityConverter()
        {
            TrueValue = ScrollBarVisibility.Visible;
            FalseValue = ScrollBarVisibility.Hidden;
        }
    }

    public class BoolToTextWrapConverter : BoolValueConverter<TextWrapping>
    {
        public BoolToTextWrapConverter()
        {
            TrueValue = TextWrapping.Wrap;
            FalseValue = TextWrapping.NoWrap;
        }
    }

    public class BoolToVisibilityConverter : BoolValueConverter<Visibility>
    {
        public BoolToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }
    }

    public class BoolToLaunchSetupConverter : BoolValueInvertedConverter<LaunchSetup>
    {
        public BoolToLaunchSetupConverter()
        {
            TrueValue = LaunchSetup.Client;
            FalseValue = LaunchSetup.Server;
        }
    }
}
