using System.Collections.Generic;

namespace WindowsEnvironment.Model;

internal interface IParentsChainFinder
{
    List<Panel> FindChain(string findPanelName);
}

internal class ParentsChainFinder : IParentsChainFinder
{
    private readonly IPanelCollection _panels;

    public ParentsChainFinder(IPanelCollection panels)
    {
        _panels = panels;
    }

    // TODO сделать List<LayoutPanel>, возвращать только список родительских панелей
    public List<Panel> FindChain(string findPanelName)
    {
        var parentChain = new List<Panel>();
        FindParentsChain(_panels.RootPanel, findPanelName, parentChain);

        return parentChain;
    }

    private bool FindParentsChain(Panel parentPanel, string findPanelName, List<Panel> parentChain)
    {
        if (parentPanel.Name == findPanelName)
        {
            parentChain.Add(parentPanel);
            return true;
        }
        else if (parentPanel is LayoutPanel parentLayoutPanel)
        {
            foreach (var child in parentLayoutPanel.ChildrenList)
            {
                if (FindParentsChain(child, findPanelName, parentChain))
                {
                    parentChain.Add(parentLayoutPanel);
                    return true;
                }
            }
        }

        return false;
    }
}
