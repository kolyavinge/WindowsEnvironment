using System.Collections.Generic;
using DependencyInjection;

namespace WindowsEnvironment.Model;

internal class FlexWindowsEnvironment : IFlexWindowsEnvironment
{
    [Inject]
    public IPanelCollection? Panels { get; set; }

    [Inject]
    public ISetPanelPositionAction? SetPanelPositionAction { get; set; }

    [Inject]
    public ISelectTabAction? SelectTabAction { get; set; }

    [Inject]
    public IRemoveTabAction? RemoveTabAction { get; set; }

    [Inject]
    public IChangeSizePanelAction? ChangeSizePanelAction { get; set; }

    public IEvents Events { get; private set; }

    public IPanel RootPanel => Panels!.RootPanel;

    public IEnumerable<IPanel> AllPanels => Panels!;

    public FlexWindowsEnvironment(IEventsInternal events)
    {
        Events = events;
    }

    public IPanel GetPanelByName(string name) => Panels!.GetPanelByName(name);

    public (IPanel, IContentTab) GetTabByName(string name) => Panels!.GetTabByName(name);

    public (IPanel, IContentTab) GetTabById(object id) => Panels!.GetTabById(id);

    public int GetChildPanelIndex(string parentPanelName, string childPanelName) => Panels!.GetChildPanelIndex(parentPanelName, childPanelName);

    public (IPanel, IContentTab) SetPanelPosition(string panelName, PanelPosition position, Content configuration) => SetPanelPositionAction!.SetPanelPosition(panelName, position, configuration);

    public void SelectTab(string tabName) => SelectTabAction!.SelectTab(tabName);

    public void RemoveTab(string tabName, RemoveTabMode mode) => RemoveTabAction!.RemoveTab(tabName, mode);

    public double? GetPanelSize(string panelName) => ChangeSizePanelAction!.GetPanelSize(panelName);

    public void SetPanelSize(string panelName, double? size) => ChangeSizePanelAction!.SetPanelSize(panelName, size);

    public IFlexWindowsEnvironmentReader MakeReader() => new FlexWindowsEnvironmentReader(this);
}
