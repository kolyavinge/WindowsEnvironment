using System.Linq;
using WindowsEnvironment.Utils;

namespace WindowsEnvironment.Model;

public enum RemoveTabMode { Close, Unset }

internal interface IRemoveTabAction
{
    void RemoveTab(string panelName, string tabName, RemoveTabMode mode);
}

internal class RemoveTabAction : IRemoveTabAction
{
    private readonly IPanelCollectionInternal _panels;
    private readonly IParentsChainFinder _parentsChainFinder;
    private readonly IEventsInternal _events;

    public RemoveTabAction(
        IPanelCollectionInternal panels,
        IParentsChainFinder parentsChainFinder,
        IEventsInternal events)
    {
        _panels = panels;
        _parentsChainFinder = parentsChainFinder;
        _events = events;
    }

    public void RemoveTab(string panelName, string tabName, RemoveTabMode mode)
    {
        var tabPanel = _panels.GetPanelByName(panelName);
        var tab = tabPanel.Tabs.FirstOrDefault(x => x.Name == tabName) ?? throw new ArgumentException($"'{panelName}' does not contain '{tabName}'.");
        tabPanel.Tabs.Remove(tab);
        RemovedPanel? removedPanel = null;
        if (!tabPanel.IsMain && !tabPanel.Tabs.Any())
        {
            var parentsChain = _parentsChainFinder.FindChain(panelName);
            var parentPanel = parentsChain.FirstOrDefault(x => x.Children.Count > 1) ?? _panels.RootPanel;
            if (parentPanel != null)
            {
                var removed = parentsChain.GetBefore(parentPanel)!;
                parentPanel.Children.Remove(removed);
                removedPanel = new RemovedPanel(parentPanel, removed);
            }
        }
        _events.RaiseTabRemoved(removedPanel, tabPanel, tab, mode);
    }
}
