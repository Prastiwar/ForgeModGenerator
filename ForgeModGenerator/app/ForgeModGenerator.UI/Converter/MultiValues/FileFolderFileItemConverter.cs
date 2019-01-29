using ForgeModGenerator.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ForgeModGenerator.Converter
{
    public class FileFolderFileItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => new Tuple<IFileFolder, IFileItem>(values[0] as IFileFolder, values[1] as IFileItem);
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => value is Tuple<IFileFolder, IFileItem> tuple ? new object[] { tuple.Item1, tuple.Item2 } : null;
    }
}
