using System;
using System.Globalization;
using System.Windows.Data;

namespace Vision.Wpf.Converters
{
    public class ShortNameConverter : IValueConverter
    {
        private const int MaxLength = 64;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = value?.ToString();
            return name.Shorten(MaxLength);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
