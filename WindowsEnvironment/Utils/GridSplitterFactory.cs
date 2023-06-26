using System.Windows;
using System.Windows.Controls;
using WindowsEnvironment.Model;

namespace WindowsEnvironment.Utils;

internal static class GridSplitterFactory
{
    public static GridSplitter MakeSplitter(SplitOrientation orientation)
    {
        if (orientation == SplitOrientation.ByRows)
        {
            return MakeHorizontal();
        }
        else
        {
            return MakeVertical();
        }
    }

    public static GridSplitter MakeHorizontal()
    {
        return new()
        {
            Height = Constants.SplitterWidthHeight,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    public static GridSplitter MakeVertical()
    {
        return new()
        {
            Width = Constants.SplitterWidthHeight,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Stretch
        };
    }
}
