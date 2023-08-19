namespace WindowsEnvironment.Model;

internal interface ISelectTabAction
{
    void SelectTab(string tabName);
}

internal class SelectTabAction : ISelectTabAction
{
    private readonly IPanelCollection _panels;
    private readonly IEventsInternal _events;

    public SelectTabAction(IPanelCollection panels, IEventsInternal events)
    {
        _panels = panels;
        _events = events;
    }

    public void SelectTab(string tabName)
    {
        var (panel, tab) = _panels.GetTabByName(tabName);
        panel.SelectedTabName = tab.Name;
        _events.RaiseTabSelected(panel, tab);
    }
}
