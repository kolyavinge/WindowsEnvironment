using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WindowsEnvironment.Model;

internal interface IPanelCollection : IEnumerable<Panel>
{
    LayoutPanel RootPanel { get; }
    IReadOnlyCollection<ContentPanel> FlexPanels { get; }
    Panel GetPanelByName(string name);
    (ContentPanel, ContentTab) GetTabByName(string name);
    (ContentPanel, ContentTab) GetTabById(object id);
    int GetChildPanelIndex(string parentPanelName, string childPanelName);
    void SetRoot(LayoutPanel root);
    void AddFlexPanel(ContentPanel panel);
    void RemoveFlexPanelTabById(object id);
}

internal class PanelCollection : IPanelCollection
{
    private readonly List<ContentPanel> _flexPanels;

    public LayoutPanel RootPanel { get; private set; }

    public IReadOnlyCollection<ContentPanel> FlexPanels => _flexPanels;

    public PanelCollection(IPanelFactory panelFactory)
    {
        _flexPanels = new List<ContentPanel>();
        var mainPanel = panelFactory.MakeNewContentPanel();
        RootPanel = panelFactory.MakeNewLayoutPanel();
        RootPanel.ChildrenList.Add(mainPanel);
    }

    public Panel GetPanelByName(string name)
    {
        return this.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentException($"'{name}' does not exist.");
    }

    public (ContentPanel, ContentTab) GetTabByName(string name)
    {
        foreach (var panel in this.OfType<ContentPanel>())
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

    public (ContentPanel, ContentTab) GetTabById(object id)
    {
        foreach (var panel in this.OfType<ContentPanel>())
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
        var parentPanel = (LayoutPanel)GetPanelByName(parentPanelName); // TODO check LayoutPanel, else throw exception
        var childPanel = GetPanelByName(childPanelName);

        return parentPanel.ChildrenList.IndexOf(childPanel);
    }

    public void SetRoot(LayoutPanel root)
    {
        RootPanel = root;
    }

    public void AddFlexPanel(ContentPanel panel)
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

    public (ContentPanel?, ContentTab?) GetFlexPanelById(object id)
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

    public IEnumerator<Panel> GetEnumerator()
    {
        var result = new List<Panel> { RootPanel };
        result.AddRange(RootPanel.GetAllChildren());
        result.AddRange(_flexPanels);

        return result.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
