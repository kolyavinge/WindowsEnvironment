using System.Windows;
using WindowsEnvironment.View;

namespace WindowsEnvironment.Utils;

internal static class WindowCollectionExt
{
    public static void EachFlexWindow(this WindowCollection wc, Action<FlexWindow> action)
    {
        foreach (Window w in wc)
        {
            if (w is FlexWindow fw) action(fw);
        }
    }
}
