using System.Windows;
using System.Windows.Media;

namespace WindowsEnvironment.View;

public interface IFlexWindowsEnvironmentStyles
{
    Style MainPanelTabControlStyle { get; }
    Style PanelTabControlStyle { get; }
    Style HorizontalSplitterStyle { get; }
    Style VerticalSplitterStyle { get; }
    Brush FlexWindowHeaderBackground { get; }
    Brush FlexWindowHeaderForeground { get; }
    Brush PositionMarksBackground { get; }
    Brush HighlightedMarkPositionBackground { get; }
}

public partial class FlexWindowsEnvironmentView : IFlexWindowsEnvironmentStyles
{
    public Style MainPanelTabControlStyle
    {
        get { return (Style)GetValue(MainPanelTabControlStyleProperty); }
        set { SetValue(MainPanelTabControlStyleProperty, value); }
    }

    public static readonly DependencyProperty MainPanelTabControlStyleProperty =
        DependencyProperty.Register("MainPanelTabControlStyle", typeof(Style), typeof(FlexWindowsEnvironmentView));

    public Style PanelTabControlStyle
    {
        get { return (Style)GetValue(PanelTabControlStyleProperty); }
        set { SetValue(PanelTabControlStyleProperty, value); }
    }

    public static readonly DependencyProperty PanelTabControlStyleProperty =
        DependencyProperty.Register("PanelTabControlStyle", typeof(Style), typeof(FlexWindowsEnvironmentView));

    public Style HorizontalSplitterStyle
    {
        get { return (Style)GetValue(HorizontalSplitterStyleProperty); }
        set { SetValue(HorizontalSplitterStyleProperty, value); }
    }

    public static readonly DependencyProperty HorizontalSplitterStyleProperty =
        DependencyProperty.Register("HorizontalSplitterStyle", typeof(Style), typeof(FlexWindowsEnvironmentView));

    public Style VerticalSplitterStyle
    {
        get { return (Style)GetValue(VerticalSplitterStyleProperty); }
        set { SetValue(VerticalSplitterStyleProperty, value); }
    }

    public static readonly DependencyProperty VerticalSplitterStyleProperty =
        DependencyProperty.Register("VerticalSplitterStyle", typeof(Style), typeof(FlexWindowsEnvironmentView));

    public Brush FlexWindowHeaderBackground
    {
        get { return (Brush)GetValue(FlexWindowHeaderBackgroundProperty); }
        set { SetValue(FlexWindowHeaderBackgroundProperty, value); }
    }

    public static readonly DependencyProperty FlexWindowHeaderBackgroundProperty =
        DependencyProperty.Register("FlexWindowHeaderBackground", typeof(Brush), typeof(FlexWindowsEnvironmentView));

    public Brush FlexWindowHeaderForeground
    {
        get { return (Brush)GetValue(FlexWindowHeaderForegroundProperty); }
        set { SetValue(FlexWindowHeaderForegroundProperty, value); }
    }

    public static readonly DependencyProperty FlexWindowHeaderForegroundProperty =
        DependencyProperty.Register("FlexWindowHeaderForeground", typeof(Brush), typeof(FlexWindowsEnvironmentView));

    public Brush PositionMarksBackground
    {
        get { return (Brush)GetValue(PositionMarksBackgroundProperty); }
        set { SetValue(PositionMarksBackgroundProperty, value); }
    }

    public static readonly DependencyProperty PositionMarksBackgroundProperty =
        DependencyProperty.Register("PositionMarksBackground", typeof(Brush), typeof(FlexWindowsEnvironmentView));

    public Brush HighlightedMarkPositionBackground
    {
        get { return (Brush)GetValue(HighlightedMarkPositionBackgroundProperty); }
        set { SetValue(HighlightedMarkPositionBackgroundProperty, value); }
    }

    public static readonly DependencyProperty HighlightedMarkPositionBackgroundProperty =
        DependencyProperty.Register("HighlightedMarkPositionBackground", typeof(Brush), typeof(FlexWindowsEnvironmentView));
}
