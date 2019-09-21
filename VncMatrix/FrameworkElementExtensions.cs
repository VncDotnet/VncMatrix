using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace VncMatrix
{
    public static class FrameworkElementExtensions
    {
        public static (FrameworkElement parent, object? dataContext)? ParentDataContext(this FrameworkElement element)
        {
            while (true)
            {
                var doParent = VisualTreeHelper.GetParent(element);
                if (!(doParent is FrameworkElement parent))
                    return null;

                if (parent.DataContext.GetType() != element.DataContext.GetType())
                    return (parent, parent.DataContext);

                return parent.ParentDataContext();
            }
        }
    }
}
