using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Vision.BL.Lib
{
    public static class WpfHelper
    {
        public static T GetParentOfType<T>(DependencyObject obj) where T : class
        {
            var parent = VisualTreeHelper.GetParent(obj);
            if (parent != null)
            {
                if (parent is T)
                {
                    return parent as T;
                }
                else
                {
                    return GetParentOfType<T>(parent);
                }
            }
            return null;
        }
    }
}
