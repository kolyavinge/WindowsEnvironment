using WindowsEnvironment.Utils;

namespace WindowsEnvironment.Model;

internal interface ISetPanelPositionAction
{
    (Panel, ContentTab) SetPanelPosition(string panelName, PanelPosition position, Content content);
}

internal class SetPanelPositionAction : ISetPanelPositionAction
{
    private readonly IPanelCollection _panels;
    private readonly IPanelFactory _panelFactory;
    private readonly IEventsInternal _events;

    public SetPanelPositionAction(
        IPanelCollection panels,
        IPanelFactory panelFactory,
        IEventsInternal events)
    {
        _panels = panels;
        _panelFactory = panelFactory;
        _events = events;
    }

    public (Panel, ContentTab) SetPanelPosition(string panelName, PanelPosition position, Content content)
    {
        _panels.RemoveFlexPanelTabById(content.Id);
        var panel = _panels.GetPanelByName(panelName);
        if (position != PanelPosition.Middle)
        {
            var parent = panel.ParentPanel;
            if (parent == null || !parent.IsSuitableOrientation(position))
            {
                parent = _panelFactory.MakeNew();
                parent.SetOrientation(position);
                ChangeParent(parent, panel);
                _events.RaiseParentChanged(parent, panel);
            }
            var childPanel = _panelFactory.MakeNew();
            childPanel.ParentPanel = parent;
            var childPanelIndex = parent.ChildrenList.IndexOf(panel);
            if (position is PanelPosition.Right or PanelPosition.Bottom) childPanelIndex++;
            parent.ChildrenList.Insert(childPanelIndex, childPanel);
            var tab = childPanel.TabCollection.Add(content);
            _events.RaisePanelAdded(parent, childPanel, tab);

            return (childPanel, tab);
        }
        else
        {
            if (!panel.AllowTabs) throw new ArgumentException($"{panelName} does not contain tabs.");
            var tab = panel.TabCollection.Add(content);
            _events.RaiseTabAdded(panel, tab);

            return (panel, tab);
        }
    }

    private void ChangeParent(Panel parent, Panel child)
    {
        var oldParent = child.ParentPanel;
        if (oldParent != null)
        {
            oldParent.ChildrenList.Replace(child, parent);
        }
        else
        {
            _panels.SetRoot(parent);
        }
        child.ParentPanel = parent;
        parent.ParentPanel = oldParent;
        parent.ChildrenList.Add(child);
    }
}
