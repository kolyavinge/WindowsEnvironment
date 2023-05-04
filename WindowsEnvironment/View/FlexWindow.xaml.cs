using System.Windows;
using System.Windows.Input;

namespace WindowsEnvironment.View;

internal partial class FlexWindow : Window
{
    public IInputElement? _flexEnvironment;
    private bool _needToCapture;
    private Point? _lastMousePosition;

    public event EventHandler<MouseButtonEventArgs>? HeaderMouseUp;
    public event EventHandler<EventArgs>? WindowMoved;

    public UIElement MainContent
    {
        get => _contentGrid.Children[0];
        set => _contentGrid.Children.Add(value);
    }

    public FlexWindow(IInputElement? flexEnvironment = null)
    {
        InitializeComponent();
        _flexEnvironment = flexEnvironment;
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
