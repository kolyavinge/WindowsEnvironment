using System.Collections.Generic;

namespace WindowsEnvironment.Utils;

internal static class ListExt
{
    public static void Replace<T>(this List<T> list, T oldItem, T newItem)
    {
        var index = list.IndexOf(oldItem);
        list.Remove(oldItem);
        list.Insert(index, newItem);
    }
}
