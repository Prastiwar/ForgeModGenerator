using ForgeModGenerator.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converter
{
    public class SoundEventSoundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => new Tuple<SoundEvent, Sound>(values[0] as SoundEvent, values[1] as Sound);
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => value is Tuple<SoundEvent, Sound> tuple ? new object[] { tuple.Item1, tuple.Item2 } : null;
    }
}
