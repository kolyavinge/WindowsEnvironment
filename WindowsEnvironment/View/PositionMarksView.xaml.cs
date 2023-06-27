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
    #region Styles
    public Brush PositionMarksBackground
    {
        get { return (Brush)GetValue(PositionMarksBackgroundProperty); }
        set { SetValue(PositionMarksBackgroundProperty, value); }
    }

    public static readonly DependencyProperty PositionMarksBackgroundProperty =
        DependencyProperty.Register("PositionMarksBackground", typeof(Brush), typeof(PositionMarksView), new PropertyMetadata(Brushes.LightBlue));

    public Brush HighlightedPositionBackground
    {
        get { return (Brush)GetValue(HighlightedPositionBackgroundProperty); }
        set { SetValue(HighlightedPositionBackgroundProperty, value); }
    }

    public static readonly DependencyProperty HighlightedPositionBackgroundProperty =
        DependencyProperty.Register("HighlightedPositionBackground", typeof(Brush), typeof(PositionMarksView), new PropertyMetadata(Brushes.LightBlue));
    #endregion

    public string PanelName { get; }

    public PositionMarksView(string panelName = "")
    {
        InitializeComponent();
        PanelName = panelName;
    }

    public void ActivatePosition(PanelPosition position)
    {
        if (position == PanelPosition.Top)
        {
            _topPanel.Visibility = Visibility.Visible;
        }
        else if (position == PanelPosition.Left)
        {
            _leftPanel.Visibility = Visibility.Visible;
        }
        else if (position == PanelPosition.Middle)
        {
            _middlePanel.Visibility = Visibility.Visible;
        }
        else if (position == PanelPosition.Right)
        {
            _rightPanel.Visibility = Visibility.Visible;
        }
        else // Bottom
        {
            _bottomPanel.Visibility = Visibility.Visible;
        }
    }

    public void DeactivatePosition()
    {
        _topPanel.Visibility = Visibility.Hidden;
        _leftPanel.Visibility = Visibility.Hidden;
        _middlePanel.Visibility = Visibility.Hidden;
        _rightPanel.Visibility = Visibility.Hidden;
        _bottomPanel.Visibility = Visibility.Hidden;
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
