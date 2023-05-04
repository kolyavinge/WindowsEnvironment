using WindowsEnvironment.Model;

namespace WindowsEnvironment.Controllers;

public interface IMouseController
{
    void OnTabButtonDown(string panelName, string tabName, double x, double y);
    void OnWindowMouseMove(double x, double y);
    void OnWindowButtonUp();
    void OnTabMiddleButtonPress(string panelName, string tabName);
}

internal class MouseController : IMouseController
{
    private static readonly object _locked = new object();

    private readonly IFlexWindowsEnvironment _model;
    private string _panelName, _tabName;
    private MousePoint? _lastMousePoint;

    public MouseController(IFlexWindowsEnvironment model)
    {
        _panelName = _tabName = "";
        _model = model;
    }

    public void OnTabButtonDown(string panelName, string tabName, double x, double y)
    {
        _panelName = panelName;
        _tabName = tabName;
        _lastMousePoint = new MousePoint(x, y);
    }

    public void OnWindowMouseMove(double x, double y)
    {
        lock (_locked)
        {
            if (_lastMousePoint == null) return;
            var dx = _lastMousePoint.X - x;
            var dy = _lastMousePoint.Y - y;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance > 30)
            {
                _model.RemoveTab(_panelName, _tabName, RemoveTabMode.Unset);
                _lastMousePoint = null;
            }
        }
    }

    public void OnWindowButtonUp()
    {
        _lastMousePoint = null;
    }

    public void OnTabMiddleButtonPress(string panelName, string tabName)
    {
        _model.RemoveTab(panelName, tabName, RemoveTabMode.Close);
    }
}

record MousePoint(double X, double Y);
