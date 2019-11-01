using System;
using System.Globalization;
using System.Windows.Data;
using Vision.Wpf.Model;

namespace Vision.Wpf.Converters
{
    public class FolderIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var node = (NodeView)value;
            return node.NodeType == NodeViewType.Folder ? "📁" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
