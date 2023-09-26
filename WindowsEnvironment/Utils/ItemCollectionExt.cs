using System.Windows.Controls;

namespace WindowsEnvironment.Utils;

internal static class ItemCollectionExt
{
    public static TabItem? GetByName(this ItemCollection collection, string name)
    {
        foreach (var item in collection)
        {
            if (item is TabItem ti && ti.Name == name)
            {
                return ti;
            }
        }

        return null;
    }

    public static bool RemoveByName(this ItemCollection collection, string name)
    {
        TabItem? result = null;
        foreach (var item in collection)
        {
            if (item is TabItem ti && ti.Name == name)
            {
                result = ti;
                break;
            }
        }
        if (result is not null)
        {
            collection.Remove(result);
            return true;
        }
        else
        {
            return false;
        }
    }
}
