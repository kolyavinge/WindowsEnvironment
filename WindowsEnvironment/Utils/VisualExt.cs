using System.Windows;
using System.Windows.Media;

namespace WindowsEnvironment.Utils;

internal static class VisualExt
{
    public static Point GetPositionWithinParent(this Visual elem, Visual parent)
    {
        var parentPosition = parent.PointToScreen(new());
        var elemPosition = elem.PointToScreen(new());

        return new(elemPosition.X - parentPosition.X, elemPosition.Y - parentPosition.Y);
    }
}
