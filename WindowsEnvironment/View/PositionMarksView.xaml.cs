using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsEnvironment.Model;
using WindowsEnvironment.Utils;

namespace WindowsEnvironment.View;

internal class SelectedPositionResult
{
    public string PanelName { get; }
    public PanelPosition Position { get; }

    public SelectedPositionResult(string panelName, PanelPosition position)
    {
        PanelName = panelName;
        Position = position;
    }
}

internal partial class PositionMarksView : UserControl
{
    public string PanelName { get; }

    public PositionMarksView(string panelName = "")
    {
        InitializeComponent();
        PanelName = panelName;
    }

    public SelectedPositionResult? GetSelectedPosition(FrameworkElement flexEnvironment)
    {
        var mousePosition = Mouse.GetPosition(flexEnvironment);
        if (IsSelected(_topMark, mousePosition, flexEnvironment)) return new(PanelName, PanelPosition.Top);
        if (IsSelected(_leftMark, mousePosition, flexEnvironment)) return new(PanelName, PanelPosition.Left);
        if (IsSelected(_middleMark, mousePosition, flexEnvironment)) return new(PanelName, PanelPosition.Middle);
        if (IsSelected(_rightMark, mousePosition, flexEnvironment)) return new(PanelName, PanelPosition.Right);
        if (IsSelected(_bottomMark, mousePosition, flexEnvironment)) return new(PanelName, PanelPosition.Bottom);

        return null;
    }

    private bool IsSelected(Rectangle rect, Point mousePosition, Visual flexEnvironment)
    {
        var pos = rect.GetPositionWithinParent(flexEnvironment);

        return
            pos.X <= mousePosition.X && mousePosition.X <= pos.X + rect.Width &&
            pos.Y <= mousePosition.Y && mousePosition.Y <= pos.Y + rect.Height;
    }
}
