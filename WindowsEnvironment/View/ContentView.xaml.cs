using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WindowsEnvironment.View;

public partial class ContentView : UserControl, IDisposable
{
    public event MouseButtonEventHandler? HeaderMouseDown;
    public event MouseEventHandler? HeaderMouseMove;
    public event MouseButtonEventHandler? HeaderMouseUp;
    public event RoutedEventHandler? CloseButtonClick;

    #region HeaderBackground
    public Brush HeaderBackground
    {
        get => (Brush)GetValue(HeaderBackgroundyProperty);
        set => SetValue(HeaderBackgroundyProperty, value);
    }

    public static readonly DependencyProperty HeaderBackgroundyProperty =
        DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(ContentView), new PropertyMetadata(Brushes.Black));
    #endregion

    #region HeaderForeground
    public Brush HeaderForeground
    {
        get => (Brush)GetValue(HeaderForegroundProperty);
        set => SetValue(HeaderForegroundProperty, value);
    }

    public static readonly DependencyProperty HeaderForegroundProperty =
        DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(ContentView), new PropertyMetadata(Brushes.White));
    #endregion

    #region HeaderText
    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register("HeaderText", typeof(string), typeof(ContentView), new PropertyMetadata(""));
    #endregion

    #region ContentElement
    public UIElement? ContentElement
    {
        get { return (UIElement?)GetValue(ContentElementProperty); }
        set { SetValue(ContentElementProperty, value); }
    }

    public static readonly DependencyProperty ContentElementProperty =
        DependencyProperty.Register("ContentElement", typeof(UIElement), typeof(ContentView), new PropertyMetadata(PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ContentView)d;
        var contentElement = (UIElement)e.NewValue;
        control._contentGrid.Children.Clear();
        if (contentElement != null) control._contentGrid.Children.Add(contentElement);
    }
    #endregion

    public ContentView()
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

    public void Dispose()
    {
        ContentElement = null;
    }
}
