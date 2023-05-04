using System.Collections.Generic;

namespace WindowsEnvironment.Utils;

internal static class EnumerableExt
{
    public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
        {
            action(item);
        }
    }

    public static T? GetBefore<T>(this IEnumerable<T> collection, T item)
    {
        T? result = default;
        foreach (var x in collection)
        {
            if (item?.Equals(x) ?? false)
            {
                return result;
            }
            else
            {
                result = x;
            }
        }

        return result;
    }

    public static T? GetAfter<T>(this IEnumerable<T> collection, T item)
    {
        var find = false;
        foreach (var x in collection)
        {
            if (find)
            {
                return x;
            }
            else
            {
                find = item?.Equals(x) ?? false;
            }
        }

        return default;
    }
}
