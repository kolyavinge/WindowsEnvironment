using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public interface IFlexWindowsEnvironment
{
    IEvents Events { get; }
    Panel RootPanel { get; }
    IEnumerable<Panel> AllPanels { get; }
    Panel GetPanelByName(string name);
    int GetChildPanelIndex(string parentPanelName, string childPanelName);
    void SetPanelPosition(string panelName, PanelPosition position, object content);
    void RemoveTab(string panelName, string tabName, RemoveTabMode mode);
}

internal class FlexWindowsEnvironment : IFlexWindowsEnvironment
{
    private readonly IPanelCollectionInternal _panels;
    private readonly ISetPanelPositionAction _setPanelPositionAction;
    private readonly IRemoveTabAction _removeTabAction;
    private readonly IEventsInternal _events;

    public IEvents Events => _events;

    public Panel RootPanel => _panels.RootPanel;

    public IEnumerable<Panel> AllPanels => _panels;

    public FlexWindowsEnvironment(
        IPanelCollectionInternal panels,
        ISetPanelPositionAction setPanelPositionAction,
        IRemoveTabAction removeTabAction,
        IEventsInternal events)
    {
        _panels = panels;
        _setPanelPositionAction = setPanelPositionAction;
        _removeTabAction = removeTabAction;
        _events = events;
    }

    public Panel GetPanelByName(string name)
    {
        return _panels.GetPanelByName(name);
    }

    public int GetChildPanelIndex(string parentPanelName, string childPanelName)
    {
        return _panels.GetChildPanelIndex(parentPanelName, childPanelName);
    }

    public void SetPanelPosition(string panelName, PanelPosition position, object content)
    {
        _setPanelPositionAction.SetPanelPosition(panelName, position, content);
    }

    public void RemoveTab(string panelName, string tabName, RemoveTabMode mode)
    {
        _removeTabAction.RemoveTab(panelName, tabName, mode);
    }
}
