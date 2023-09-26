using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WindowsEnvironment.Utils;

internal static class GridExt
{
    public static T FindChildRec<T>(this Grid parent, string childName) where T : FrameworkElement
    {
        return parent.FindChildRecOrDefault<T>(childName) ?? throw new ArgumentException($"Item {childName} has not found.");
    }

    public static T? FindChildRecOrDefault<T>(this Grid parent, string childName) where T : FrameworkElement
    {
        T? foundChild = default;
        foreach (var child in parent.Children)
        {
            if (child is T item && item?.Name == childName)
            {
                return item;
            }
            else if (child is Grid childGrid)
            {
                foundChild = childGrid.FindChildRecOrDefault<T>(childName);
                if (foundChild is not null)
                {
                    return foundChild;
                }
            }
        }

        return foundChild;
    }

    public static IEnumerable<T> FindChildren<T>(this Grid parent) where T : FrameworkElement
    {
        foreach (var child in parent.Children)
        {
            if (child is T item)
            {
                yield return item;
            }
        }
    }
}
