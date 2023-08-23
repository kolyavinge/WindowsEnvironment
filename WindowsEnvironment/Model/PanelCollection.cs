using System.Collections;
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
    void AddFlexPanel(Panel panel);
    void RemoveFlexPanelTabById(object id);
}

internal class PanelCollection : IPanelCollection
{
    private readonly List<Panel> _flexPanels;

    public Panel RootPanel { get; private set; }

    public IReadOnlyCollection<Panel> FlexPanels => _flexPanels;

    public PanelCollection(IPanelFactory panelFactory)
    {
        _flexPanels = new List<Panel>();
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
            foreach (var tab in panel.TabCollection)
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
            foreach (var tab in panel.TabCollection)
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

    public void AddFlexPanel(Panel panel)
    {
        _flexPanels.Add(panel);
    }

    public void RemoveFlexPanelTabById(object id)
    {
        var (flexPanel, tab) = GetFlexPanelById(id);
        if (flexPanel != null)
        {
            flexPanel.TabCollection.Remove(tab!);
            if (!flexPanel.TabCollection.Any())
            {
                _flexPanels.Remove(flexPanel);
            }
        }
    }

    public (Panel?, ContentTab?) GetFlexPanelById(object id)
    {
        foreach (var flexPanel in _flexPanels)
        {
            foreach (var tab in flexPanel.TabCollection)
            {
                if (tab.Content.Id == id)
                {
                    return (flexPanel, tab);
                }
            }
        }

        return (default, default);
    }

    public IEnumerator<Panel> GetEnumerator() => new[] { RootPanel }.Union(RootPanel.GetAllChildren()).Union(_flexPanels).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
