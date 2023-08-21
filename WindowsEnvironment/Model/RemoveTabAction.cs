using System.Linq;
using WindowsEnvironment.Utils;

namespace WindowsEnvironment.Model;

public enum RemoveTabMode { Close, Unset }

internal interface IRemoveTabAction
{
    void RemoveTab(string tabName, RemoveTabMode mode);
}

internal class RemoveTabAction : IRemoveTabAction
{
    private readonly IPanelFactory _panelFactory;
    private readonly IPanelCollection _panels;
    private readonly IParentsChainFinder _parentsChainFinder;
    private readonly IEventsInternal _events;

    public RemoveTabAction(
        IPanelFactory panelFactory,
        IPanelCollection panels,
        IParentsChainFinder parentsChainFinder,
        IEventsInternal events)
    {
        _panelFactory = panelFactory;
        _panels = panels;
        _parentsChainFinder = parentsChainFinder;
        _events = events;
    }

    public void RemoveTab(string tabName, RemoveTabMode mode)
    {
        var (panel, tab) = _panels.GetTabByName(tabName);
        panel.ContentTabCollection.Remove(tab);
        RemovedPanelInfo? removedPanelInfo = null;
        if (!panel.IsMain && !panel.ContentTabCollection.Any())
        {
            var parentsChain = _parentsChainFinder.FindChain(panel.Name);
            var parentPanel = parentsChain.FirstOrDefault(x => x.ChildrenCollection.Count > 1) ?? _panels.RootPanel;
            if (parentPanel != null)
            {
                var removedPanel = parentsChain.GetBefore(parentPanel)!;
                parentPanel.ChildrenCollection.Remove(removedPanel);
                removedPanelInfo = new RemovedPanelInfo(parentPanel, removedPanel);
            }
        }
        if (removedPanelInfo != null)
        {
            var lastChild = removedPanelInfo.Parent.Children.LastOrDefault();
            if (lastChild != null) lastChild.Size = null;
        }
        _events.RaiseTabRemoved(removedPanelInfo, panel, tab, mode);
        if (mode == RemoveTabMode.Close && tab.Content.CloseCallback != null)
        {
            tab.Content.CloseCallback();
        }
        if (mode == RemoveTabMode.Unset)
        {
            var flexPanel = _panelFactory.MakeNew();
            flexPanel.State = PanelState.Flex;
            flexPanel.ContentTabCollection.Add(tab.Content);
            _panels.AddFlexPanel(flexPanel);
        }
    }
}
