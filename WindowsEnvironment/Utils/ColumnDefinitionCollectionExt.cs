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
}
