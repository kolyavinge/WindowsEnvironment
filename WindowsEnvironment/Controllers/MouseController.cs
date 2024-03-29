﻿using WindowsEnvironment.Model;

namespace WindowsEnvironment.Controllers;

public interface IMouseController
{
    void OnTabHeaderButtonDown(string tabName, double x, double y);
    void OnTabHeaderMouseMove(double x, double y);
    void OnTabHeaderButtonUp();
    void OnTabHeaderMiddleButtonPress(string tabName);
}

internal class MouseController : IMouseController
{
    private readonly IFlexWindowsEnvironment _model;
    private string _tabName;
    private MousePoint? _lastMousePoint;

    public MouseController(IFlexWindowsEnvironment model)
    {
        _tabName = "";
        _model = model;
    }

    public void OnTabHeaderButtonDown(string tabName, double x, double y)
    {
        _tabName = tabName;
        _lastMousePoint = new MousePoint(x, y);
    }

    public void OnTabHeaderMouseMove(double x, double y)
    {
        if (_lastMousePoint is null) return;
        var dx = _lastMousePoint.X - x;
        var dy = _lastMousePoint.Y - y;
        var distance = Math.Sqrt(dx * dx + dy * dy);
        if (distance > 30)
        {
            _lastMousePoint = null;
            _model.RemoveTab(_tabName, RemoveTabMode.Unset);
        }
    }

    public void OnTabHeaderButtonUp()
    {
        _lastMousePoint = null;
    }

    public void OnTabHeaderMiddleButtonPress(string tabName)
    {
        _model.RemoveTab(tabName, RemoveTabMode.Close);
    }
}

record MousePoint(double X, double Y);
