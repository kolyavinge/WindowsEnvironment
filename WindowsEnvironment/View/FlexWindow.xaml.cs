using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WindowsEnvironment.Model;

namespace WindowsEnvironment.View;

internal partial class FlexWindow : Window
{
    #region Styles
    public Brush HeaderBackground
    {
        get { return (Brush)GetValue(HeaderBackgroundProperty); }
        set { SetValue(HeaderBackgroundProperty, value); }
    }

    public static readonly DependencyProperty HeaderBackgroundProperty =
        DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(FlexWindow), new PropertyMetadata(Brushes.Gray));

    public Brush HeaderForeground
    {
        get { return (Brush)GetValue(HeaderForegroundProperty); }
        set { SetValue(HeaderForegroundProperty, value); }
    }

    public static readonly DependencyProperty HeaderForegroundProperty =
        DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(FlexWindow), new PropertyMetadata(Brushes.Black));
    #endregion

    private readonly IInputElement? _flexEnvironment;
    private bool _needToCapture;
    private Point? _lastMousePosition;

    public event EventHandler<MouseButtonEventArgs>? HeaderMouseUp;
    public event EventHandler<EventArgs>? WindowMoved;

    public UIElement MainContent => _contentGrid.Children[0];

    public FlexWindow()
    {
        InitializeComponent();
    }

    public FlexWindow(
        IInputElement flexEnvironment, ContentTab contentTab, TabControl tabControl, Point parentPosition, Point mousePosition)
        : this()
    {
        _flexEnvironment = flexEnvironment;
        _contentGrid.Children.Add((UIElement)contentTab.Content);
        Title = contentTab.Name;
        Width = tabControl.ActualWidth;
        Height = tabControl.ActualHeight;
        Left = parentPosition.X + mousePosition.X - Width / 2.0;
        Top = parentPosition.Y + mousePosition.Y - 20;
    }

    private void OnHeaderMouseDown(object sender, MouseButtonEventArgs e)
    {
        CaptureHeader();
        Mouse.Capture(_headerGrid);
    }

    private void OnHeaderMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _lastMousePosition != null)
        {
            var mousePosition = Mouse.GetPosition(_flexEnvironment);
            Left += mousePosition.X - _lastMousePosition.Value.X;
            Top += mousePosition.Y - _lastMousePosition.Value.Y;
            _lastMousePosition = mousePosition;
            WindowMoved?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnHeaderMouseUp(object sender, MouseButtonEventArgs e)
    {
        _lastMousePosition = null;
        Mouse.Capture(null);
        HeaderMouseUp?.Invoke(this, e);
    }

    public void CaptureHeader(Point? mousePosition = null)
    {
        _lastMousePosition = mousePosition ?? Mouse.GetPosition(_flexEnvironment);
        _needToCapture = true;
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        if (_needToCapture)
        {
            Mouse.Capture(_headerGrid);
            _needToCapture = false;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _contentGrid.Children.Clear();
        base.OnClosed(e);
    }

    private void OnWindowMouseUp(object sender, MouseButtonEventArgs e)
    {
        Mouse.Capture(null);
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
