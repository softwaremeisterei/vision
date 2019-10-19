using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Vision.Wpf.Converters
{
    public class FolderNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filepath = value?.ToString();
            return Path.GetDirectoryName(filepath);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
