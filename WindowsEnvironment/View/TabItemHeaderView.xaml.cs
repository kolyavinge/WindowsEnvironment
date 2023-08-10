using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsEnvironment.View;

public partial class TabItemHeaderView : UserControl
{
    public event MouseButtonEventHandler? HeaderMouseDown;
    public event MouseEventHandler? HeaderMouseMove;
    public event MouseButtonEventHandler? HeaderMouseUp;
    public event RoutedEventHandler? CloseButtonClick;

    #region HeaderText
    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register("HeaderText", typeof(string), typeof(TabItemHeaderView), new PropertyMetadata(""));
    #endregion

    #region IsCloseButtonVisible
    public bool IsCloseButtonVisible
    {
        get => (bool)GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }

    public static readonly DependencyProperty IsCloseButtonVisibleProperty =
        DependencyProperty.Register("IsCloseButtonVisible", typeof(bool), typeof(TabItemHeaderView), new PropertyMetadata(false));
    #endregion

    public TabItemHeaderView()
    {
        InitializeComponent();
    }

    public void CaptureHeader()
    {
        Mouse.Capture(_header);
    }

    public void ReleaseHeader()
    {
        Mouse.Capture(null);
    }

    private void OnHeaderMouseDown(object sender, MouseButtonEventArgs e)
    {
        HeaderMouseDown?.Invoke(this, e);
    }

    private void OnHeaderMouseMove(object sender, MouseEventArgs e)
    {
        HeaderMouseMove?.Invoke(this, e);
    }

    private void OnHeaderMouseUp(object sender, MouseButtonEventArgs e)
    {
        HeaderMouseUp?.Invoke(this, e);
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        CloseButtonClick?.Invoke(this, e);
    }
}
