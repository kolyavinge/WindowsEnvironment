using System.Linq;

namespace WindowsEnvironment.Model;

internal interface ISelectTabAction
{
    void SelectTab(string panelName, string tabName);
}

internal class SelectTabAction : ISelectTabAction
{
    private readonly IPanelCollectionInternal _panels;
    private readonly IEventsInternal _events;

    public SelectTabAction(IPanelCollectionInternal panels, IEventsInternal events)
    {
        _panels = panels;
        _events = events;
    }

    public void SelectTab(string panelName, string tabName)
    {
        var tabPanel = _panels.GetPanelByName(panelName);
        var tab = tabPanel.Tabs.FirstOrDefault(x => x.Name == tabName) ?? throw new ArgumentException($"'{panelName}' does not contain '{tabName}'.");
        tabPanel.SelectedTabName = tab.Name;
        _events.RaiseTabSelected(tabPanel, tab);
    }
}
