using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WindowsEnvironment.View;

internal class PositionMarksWindow : Window
{
    private readonly Grid _contentGrid;

    public IEnumerable<PositionMarksView> Marks => _contentGrid.Children.Cast<PositionMarksView>();

    public PositionMarksWindow(FrameworkElement? flexEnvironment = null)
    {
        WindowStyle = WindowStyle.None;
        Background = null;
        AllowsTransparency = true;
        ShowInTaskbar = false;
        _contentGrid = new Grid();
        if (flexEnvironment != null)
        {
            var parentPosition = flexEnvironment.PointToScreen(new());
            Left = parentPosition.X;
            Top = parentPosition.Y;
            Width = flexEnvironment.ActualWidth;
            Height = flexEnvironment.ActualHeight;
        }
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        Content = _contentGrid;
    }

    public void AddMarksFor(string panelName, Grid grid, Point gridPosition)
    {
        var marks = new PositionMarksView(panelName);
        marks.VerticalAlignment = VerticalAlignment.Top;
        marks.HorizontalAlignment = HorizontalAlignment.Left;
        marks.Margin = new(gridPosition.X, gridPosition.Y, 0, 0);
        marks.Width = grid.ActualWidth;
        marks.Height = grid.ActualHeight;
        _contentGrid.Children.Add(marks);
    }
}
