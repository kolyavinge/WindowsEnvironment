using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WindowsEnvironment.Model;

public interface IPanelCollection : IEnumerable<Panel>
{
    Panel RootPanel { get; }
    Panel GetPanelByName(string name);
    int GetChildPanelIndex(string parentPanelName, string childPanelName);
}

internal interface IPanelCollectionInternal : IPanelCollection
{
    void SetRoot(Panel root);
}

internal class PanelCollection : IPanelCollectionInternal
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

    public int GetChildPanelIndex(string parentPanelName, string childPanelName)
    {
        var parentPanel = GetPanelByName(parentPanelName);
        var childPanel = GetPanelByName(childPanelName);

        return parentPanel.Children.IndexOf(childPanel);
    }

    public void SetRoot(Panel root)
    {
        RootPanel = root;
    }

    public IEnumerator<Panel> GetEnumerator() => new[] { RootPanel }.Union(RootPanel.GetAllChildren()).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
