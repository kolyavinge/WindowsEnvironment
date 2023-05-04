using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WindowsEnvironment.Utils;

internal static class UIElementCollectionExt
{
    public static IEnumerable<UIElement> ToList(this UIElementCollection collection)
    {
        var result = new List<UIElement>();
        foreach (UIElement item in collection)
        {
            result.Add(item);
        }

        return result;
    }

    public static void AddRange(this UIElementCollection collection, IEnumerable<UIElement> range)
    {
        foreach (var item in range)
        {
            collection.Add(item);
        }
    }

    public static bool RemoveByName(this UIElementCollection collection, string name)
    {
        FrameworkElement? result = null;
        foreach (var item in collection)
        {
            if (item is FrameworkElement fe && fe.Name == name)
            {
                result = fe;
                break;
            }
        }
        if (result != null)
        {
            collection.Remove(result);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void RemoveByType<T>(this UIElementCollection collection) where T : UIElement
    {
        for (int i = 0; i < collection.Count;)
        {
            if (collection[i] is T)
            {
                collection.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}
