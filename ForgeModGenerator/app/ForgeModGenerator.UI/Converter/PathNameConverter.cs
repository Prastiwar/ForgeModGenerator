using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace ForgeModGenerator.Converter
{
    public class PathNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value as string;
            if (Directory.Exists(val))
            {
                return new DirectoryInfo(val).Name;
            }
            else if (File.Exists(val))
            {
                return new FileInfo(val).Name;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
