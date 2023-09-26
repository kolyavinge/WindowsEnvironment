using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsEnvironment.View;

internal class PositionMarksWindow : Window
{
    #region Styles
    public Brush PositionMarksBackground
    {
        get { return (Brush)GetValue(PositionMarksBackgroundProperty); }
        set { SetValue(PositionMarksBackgroundProperty, value); }
    }

    public static readonly DependencyProperty PositionMarksBackgroundProperty =
        DependencyProperty.Register("PositionMarksBackground", typeof(Brush), typeof(PositionMarksWindow));

    public Brush HighlightedPositionBackground
    {
        get { return (Brush)GetValue(HighlightedPositionBackgroundProperty); }
        set { SetValue(HighlightedPositionBackgroundProperty, value); }
    }

    public static readonly DependencyProperty HighlightedPositionBackgroundProperty =
        DependencyProperty.Register("HighlightedPositionBackground", typeof(Brush), typeof(PositionMarksWindow));
    #endregion

    private readonly Grid _contentGrid;

    public IEnumerable<PositionMarksView> Marks => _contentGrid.Children.Cast<PositionMarksView>();

    public PositionMarksWindow(FrameworkElement? flexEnvironment = null)
    {
        WindowStyle = WindowStyle.None;
        Background = null;
        AllowsTransparency = true;
        ShowInTaskbar = false;
        _contentGrid = new Grid();
        if (flexEnvironment is not null)
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
        marks.PositionMarksBackground = PositionMarksBackground;
        marks.HighlightedPositionBackground = HighlightedPositionBackground;
        _contentGrid.Children.Add(marks);
    }

    public (PositionMarksView?, SelectedPositionResult?) GetSelectedMarkAndPosition(FrameworkElement flexEnvironment)
    {
        foreach (var marks in Marks)
        {
            var selectedPosition = marks.GetSelectedPosition(flexEnvironment);
            if (selectedPosition is not null)
            {
                return (marks, selectedPosition);
            }
        }

        return (null, null);
    }

    public void DeactivateAllPositions()
    {
        foreach (var mark in Marks)
        {
            mark.DeactivatePosition();
        }
    }
}
