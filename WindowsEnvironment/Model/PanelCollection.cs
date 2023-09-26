using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WindowsEnvironment.Model;

internal interface IPanelCollection : IEnumerable<Panel>
{
    LayoutPanel RootPanel { get; }
    IReadOnlyCollection<ContentPanel> FlexPanels { get; }
    Panel GetPanelByName(string name);
    ContentTab GetTabByName(string name);
    ContentTab GetTabById(object id);
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
        RootPanel.Children.Add(mainPanel);
    }

    public Panel GetPanelByName(string name)
    {
        return this.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentException($"'{name}' does not exist.");
    }

    public ContentTab GetTabByName(string name)
    {
        foreach (var panel in this.OfType<ContentPanel>())
        {
            foreach (var tab in panel.Tab)
            {
                if (tab.Name == name)
                {
                    return tab;
                }
            }
        }

        throw new ArgumentException($"'{name}' does not exist.");
    }

    public ContentTab GetTabById(object id)
    {
        foreach (var panel in this.OfType<ContentPanel>())
        {
            foreach (var tab in panel.Tab)
            {
                if (tab.Content.Id == id)
                {
                    return tab;
                }
            }
        }

        throw new ArgumentException($"Tab with id '{id}' does not exist.");
    }

    public int GetChildPanelIndex(string parentPanelName, string childPanelName)
    {
        var parentPanel = (LayoutPanel)GetPanelByName(parentPanelName); // TODO check LayoutPanel, else throw exception
        var childPanel = GetPanelByName(childPanelName);

        return parentPanel.Children.IndexOf(childPanel);
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
        if (flexPanel is not null)
        {
            flexPanel.Tab.Remove(tab!);
            if (!flexPanel.Tab.Any())
            {
                _flexPanels.Remove(flexPanel);
            }
        }
    }

    public (ContentPanel?, ContentTab?) GetFlexPanelById(object id)
    {
        foreach (var flexPanel in _flexPanels)
        {
            foreach (var tab in flexPanel.Tab)
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
