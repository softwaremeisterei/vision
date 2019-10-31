using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Vision.BL.Model;

namespace Vision.Wpf.Converters
{
    public class FolderIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var node = (Node)value;
            return node.NodeType == NodeType.Folder ? "📁" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
