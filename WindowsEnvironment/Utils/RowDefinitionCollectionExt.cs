using System.Collections.Generic;
using System.Windows.Controls;

namespace WindowsEnvironment.Utils;

internal static class RowDefinitionCollectionExt
{
    public static void AddRange(this RowDefinitionCollection collection, IEnumerable<RowDefinition> range)
    {
        foreach (var item in range)
        {
            collection.Add(item);
        }
    }
}
