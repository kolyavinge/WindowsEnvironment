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
        else
        {
            foreach (var child in parentPanel.ChildrenCollection)
            {
                if (FindParentsChain(child, findPanelName, parentChain))
                {
                    parentChain.Add(parentPanel);
                    return true;
                }
            }
        }

        return false;
    }
}
