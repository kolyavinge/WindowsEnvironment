using System.Windows;
using System.Windows.Media;

namespace WindowsEnvironment.Utils;

internal static class DependencyObjectExt
{
    public static T FindChild<T>(this DependencyObject parent, string childName) where T : FrameworkElement
    {
        return parent.FindChildOrDefault<T>(childName) ?? throw new ArgumentException($"Item {childName} has not found.");
    }

    public static T? FindChildOrDefault<T>(this DependencyObject parent, string childName) where T : FrameworkElement
    {
        T? foundChild = default;
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is FrameworkElement fe && fe?.Name == childName)
            {
                return (T)child;
            }
            else
            {
                foundChild = child.FindChildOrDefault<T>(childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
        }

        return foundChild;
    }
}
