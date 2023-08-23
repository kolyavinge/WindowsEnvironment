using System.Windows;
using System.Windows.Controls;
using WindowsEnvironment.Model;

namespace WindowsEnvironment.Utils;

internal static class GridSplitterFactory
{
    public static GridSplitter MakeSplitter(PanelOrientation orientation, Style horizontalSplitterStyle, Style verticalSplitterStyle)
    {
        if (orientation == PanelOrientation.ByRows)
        {
            return MakeHorizontal(horizontalSplitterStyle);
        }
        else
        {
            return MakeVertical(verticalSplitterStyle);
        }
    }

    public static GridSplitter MakeHorizontal(Style style)
    {
        return new()
        {
            MinHeight = Constants.SplitterWidthHeight,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            Style = style
        };
    }

    public static GridSplitter MakeVertical(Style style)
    {
        return new()
        {
            MinWidth = Constants.SplitterWidthHeight,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Stretch,
            Style = style
        };
    }
}
