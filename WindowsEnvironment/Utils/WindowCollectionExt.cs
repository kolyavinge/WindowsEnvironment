using System.Collections.Generic;
using System.Windows;
using WindowsEnvironment.View;

namespace WindowsEnvironment.Utils;

internal static class WindowCollectionExt
{
    public static IEnumerable<FlexWindow> GetFlexWindows(this WindowCollection wc)
    {
        foreach (Window w in wc)
        {
            if (w is FlexWindow fw) yield return fw;
        }
    }

    public static void Each(this WindowCollection wc, Action<Window> action)
    {
        foreach (Window w in wc)
        {
            action(w);
        }
    }
}
