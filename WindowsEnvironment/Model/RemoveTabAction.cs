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
    private readonly IPanelCollection _panels;
    private readonly IParentsChainFinder _parentsChainFinder;
    private readonly IEventsInternal _events;

    public RemoveTabAction(
        IPanelCollection panels,
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
        var tab = tabPanel.ContentTabCollection.FirstOrDefault(x => x.Name == tabName) ?? throw new ArgumentException($"'{panelName}' does not contain '{tabName}'.");
        tabPanel.ContentTabCollection.Remove(tab);
        RemovedPanel? removedPanel = null;
        if (!tabPanel.IsMain && !tabPanel.ContentTabCollection.Any())
        {
            var parentsChain = _parentsChainFinder.FindChain(panelName);
            var parentPanel = parentsChain.FirstOrDefault(x => x.ChildrenCollection.Count > 1) ?? _panels.RootPanel;
            if (parentPanel != null)
            {
                var removed = parentsChain.GetBefore(parentPanel)!;
                parentPanel.ChildrenCollection.Remove(removed);
                removedPanel = new RemovedPanel(parentPanel, removed);
            }
        }
        if (removedPanel != null)
        {
            var lastChild = removedPanel.Parent.Children.LastOrDefault();
            if (lastChild != null) lastChild.Size = null;
        }
        _events.RaiseTabRemoved(removedPanel, tabPanel, tab, mode);
        if (mode == RemoveTabMode.Close && tab.Content.CloseCallback != null)
        {
            tab.Content.CloseCallback();
        }
    }
}
