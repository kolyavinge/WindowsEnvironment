﻿using System.Linq;
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
        panel.TabCollection.Remove(tab);
        Panel? removedPanel = null;
        if (!panel.IsMain && !panel.TabCollection.Any())
        {
            var parentsChain = _parentsChainFinder.FindChain(panel.Name);
            var parentPanel = parentsChain.First(x => x.ChildrenList.Count > 1);
            removedPanel = (Panel?)parentsChain.GetBefore(parentPanel) ?? panel;
            parentPanel.ChildrenList.Remove(removedPanel);
        }
        if (removedPanel != null)
        {
            var lastChild = removedPanel.Parent!.Children.LastOrDefault();
            if (lastChild is ContentPanel lastChildContentPanel) lastChildContentPanel.Size = null;
        }
        _events.RaiseTabRemoved(removedPanel, panel, tab, mode);
        if (mode == RemoveTabMode.Close && tab.Content.CloseCallback != null)
        {
            tab.Content.CloseCallback();
        }
        if (mode == RemoveTabMode.Unset)
        {
            var flexPanel = _panelFactory.MakeNewContentPanel();
            flexPanel.State = PanelState.Flex;
            flexPanel.TabCollection.Add(tab.Content);
            _panels.AddFlexPanel(flexPanel);
        }
    }
}
