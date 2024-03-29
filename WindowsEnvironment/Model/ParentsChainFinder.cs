﻿using System.Collections.Generic;

namespace WindowsEnvironment.Model;

internal interface IParentsChainFinder
{
    List<LayoutPanel> FindChain(string findPanelName);
}

internal class ParentsChainFinder : IParentsChainFinder
{
    private readonly IPanelCollection _panels;

    public ParentsChainFinder(IPanelCollection panels)
    {
        _panels = panels;
    }

    public List<LayoutPanel> FindChain(string findPanelName)
    {
        var parentChain = new List<LayoutPanel>();
        FindParentsChain(_panels.RootPanel, findPanelName, parentChain);

        return parentChain;
    }

    private bool FindParentsChain(Panel parentPanel, string findPanelName, List<LayoutPanel> parentChain)
    {
        if (parentPanel.Name == findPanelName)
        {
            return true;
        }
        else if (parentPanel is LayoutPanel parentLayoutPanel)
        {
            foreach (var child in parentLayoutPanel.Children)
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
