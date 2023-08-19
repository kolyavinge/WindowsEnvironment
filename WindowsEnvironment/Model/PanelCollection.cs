﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WindowsEnvironment.Model;

internal interface IPanelCollection : IEnumerable<Panel>
{
    Panel RootPanel { get; }
    Panel GetPanelByName(string name);
    (Panel, ContentTab) GetTabByName(string name);
    (Panel, ContentTab) GetTabById(object id);
    int GetChildPanelIndex(string parentPanelName, string childPanelName);
    void SetRoot(Panel root);
}

internal class PanelCollection : IPanelCollection
{
    public Panel RootPanel { get; private set; }

    public PanelCollection(IPanelFactory panelFactory)
    {
        RootPanel = panelFactory.MakeNew();
    }

    public Panel GetPanelByName(string name)
    {
        return this.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentException($"'{name}' does not exist.");
    }

    public (Panel, ContentTab) GetTabByName(string name)
    {
        foreach (var panel in this)
        {
            foreach (var tab in panel.ContentTabCollection)
            {
                if (tab.Name == name)
                {
                    return (panel, tab);
                }
            }
        }

        throw new ArgumentException($"'{name}' does not exist.");
    }

    public (Panel, ContentTab) GetTabById(object id)
    {
        foreach (var panel in this)
        {
            foreach (var tab in panel.ContentTabCollection)
            {
                if (tab.Content.Id == id)
                {
                    return (panel, tab);
                }
            }
        }

        throw new ArgumentException($"Tab with id '{id}' does not exist.");
    }

    public int GetChildPanelIndex(string parentPanelName, string childPanelName)
    {
        var parentPanel = GetPanelByName(parentPanelName);
        var childPanel = GetPanelByName(childPanelName);

        return parentPanel.ChildrenCollection.IndexOf(childPanel);
    }

    public void SetRoot(Panel root)
    {
        RootPanel = root;
    }

    public IEnumerator<Panel> GetEnumerator() => new[] { RootPanel }.Union(RootPanel.GetAllChildren()).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
