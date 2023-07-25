using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public interface IFlexWindowsEnvironment
{
    IEvents Events { get; }
    Panel RootPanel { get; }
    IEnumerable<Panel> AllPanels { get; }
    Panel GetPanelByName(string name);
    int GetChildPanelIndex(string parentPanelName, string childPanelName);
    (Panel, ContentTab) SetPanelPosition(string panelName, PanelPosition position, Content configuration);
    void SelectTab(string panelName, string tabName);
    void RemoveTab(string panelName, string tabName, RemoveTabMode mode);
    IFlexWindowsEnvironmentReader MakeReader();
}

internal class FlexWindowsEnvironment : IFlexWindowsEnvironment
{
    private readonly IPanelCollectionInternal _panels;
    private readonly ISetPanelPositionAction _setPanelPositionAction;
    private readonly ISelectTabAction _selectTabAction;
    private readonly IRemoveTabAction _removeTabAction;
    private readonly IEventsInternal _events;

    public IEvents Events => _events;

    public Panel RootPanel => _panels.RootPanel;

    public IEnumerable<Panel> AllPanels => _panels;

    public FlexWindowsEnvironment(
        IPanelCollectionInternal panels,
        ISetPanelPositionAction setPanelPositionAction,
        ISelectTabAction selectTabAction,
        IRemoveTabAction removeTabAction,
        IEventsInternal events)
    {
        _panels = panels;
        _setPanelPositionAction = setPanelPositionAction;
        _selectTabAction = selectTabAction;
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

    public (Panel, ContentTab) SetPanelPosition(string panelName, PanelPosition position, Content configuration)
    {
        return _setPanelPositionAction.SetPanelPosition(panelName, position, configuration);
    }

    public void SelectTab(string panelName, string tabName)
    {
        _selectTabAction.SelectTab(panelName, tabName);
    }

    public void RemoveTab(string panelName, string tabName, RemoveTabMode mode)
    {
        _removeTabAction.RemoveTab(panelName, tabName, mode);
    }

    public IFlexWindowsEnvironmentReader MakeReader()
    {
        return new FlexWindowsEnvironmentReader(this);
    }
}
