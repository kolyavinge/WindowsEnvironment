using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public interface IFlexWindowsEnvironment
{
    IEvents Events { get; }
    IPanel RootPanel { get; }
    IEnumerable<IPanel> AllPanels { get; }
    IPanel GetPanelByName(string name);
    int GetChildPanelIndex(string parentPanelName, string childPanelName);
    (IPanel, IContentTab) SetPanelPosition(string panelName, PanelPosition position, Content configuration);
    void SelectTab(string panelName, string tabName);
    void RemoveTab(string panelName, string tabName, RemoveTabMode mode);
    double? GetPanelSize(string panelName);
    void SetPanelSize(string panelName, double? size);
    IFlexWindowsEnvironmentReader MakeReader();
}

internal class FlexWindowsEnvironment : IFlexWindowsEnvironment
{
    private readonly IPanelCollectionInternal _panels;
    private readonly ISetPanelPositionAction _setPanelPositionAction;
    private readonly ISelectTabAction _selectTabAction;
    private readonly IRemoveTabAction _removeTabAction;
    private readonly IChangeSizePanelAction _changeSizePanelAction;
    private readonly IEventsInternal _events;

    public IEvents Events => _events;

    public IPanel RootPanel => _panels.RootPanel;

    public IEnumerable<IPanel> AllPanels => _panels;

    public FlexWindowsEnvironment(
        IPanelCollectionInternal panels,
        ISetPanelPositionAction setPanelPositionAction,
        ISelectTabAction selectTabAction,
        IRemoveTabAction removeTabAction,
        IChangeSizePanelAction changeSizePanelAction,
        IEventsInternal events)
    {
        _panels = panels;
        _setPanelPositionAction = setPanelPositionAction;
        _selectTabAction = selectTabAction;
        _removeTabAction = removeTabAction;
        _changeSizePanelAction = changeSizePanelAction;
        _events = events;
    }

    public IPanel GetPanelByName(string name)
    {
        return _panels.GetPanelByName(name);
    }

    public int GetChildPanelIndex(string parentPanelName, string childPanelName)
    {
        return _panels.GetChildPanelIndex(parentPanelName, childPanelName);
    }

    public (IPanel, IContentTab) SetPanelPosition(string panelName, PanelPosition position, Content configuration)
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

    public double? GetPanelSize(string panelName)
    {
        return _changeSizePanelAction.GetPanelSize(panelName);
    }

    public void SetPanelSize(string panelName, double? size)
    {
        _changeSizePanelAction.SetPanelSize(panelName, size);
    }

    public IFlexWindowsEnvironmentReader MakeReader()
    {
        return new FlexWindowsEnvironmentReader(this);
    }
}
