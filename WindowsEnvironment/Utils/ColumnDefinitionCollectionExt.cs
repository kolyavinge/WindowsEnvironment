using System.Collections.Generic;
using System.Windows.Controls;

namespace WindowsEnvironment.Utils;

internal static class ColumnDefinitionCollectionExt
{
    public static void AddRange(this ColumnDefinitionCollection collection, IEnumerable<ColumnDefinition> range)
    {
        foreach (var item in range)
        {
            collection.Add(item);
        }
    }

    public static void RemoveAll(this ColumnDefinitionCollection collection, Predicate<ColumnDefinition> condition)
    {
        for (int i = 0; i < collection.Count;)
        {
            if (condition(collection[i]))
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
