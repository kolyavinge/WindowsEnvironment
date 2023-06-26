using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WindowsEnvironment.Utils;

internal static class DependencyObjectExt
{
    public static T FindChildRec<T>(this DependencyObject parent, string childName) where T : FrameworkElement
    {
        return parent.FindChildRecOrDefault<T>(childName) ?? throw new ArgumentException($"Item {childName} has not found.");
    }

    public static T? FindChildRecOrDefault<T>(this DependencyObject parent, string childName) where T : FrameworkElement
    {
        T? foundChild = default;
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T item && item?.Name == childName)
            {
                return item;
            }
            else
            {
                foundChild = child.FindChildRecOrDefault<T>(childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
        }

        return foundChild;
    }

    public static IEnumerable<T> FindChildren<T>(this DependencyObject parent) where T : FrameworkElement
    {
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T item)
            {
                yield return item;
            }
        }
    }
}
