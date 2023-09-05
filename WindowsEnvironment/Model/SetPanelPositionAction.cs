using WindowsEnvironment.Utils;

namespace WindowsEnvironment.Model;

internal interface ISetPanelPositionAction
{
    (ContentPanel, ContentTab) SetPanelPosition(string panelName, PanelPosition position, Content content);
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

    public (ContentPanel, ContentTab) SetPanelPosition(string panelName, PanelPosition position, Content content)
    {
        _panels.RemoveFlexPanelTabById(content.Id);
        var panel = _panels.GetPanelByName(panelName);
        if (position != PanelPosition.Middle)
        {
            var parent = panel.ParentPanel;
            if (parent == null || !parent.IsSuitableOrientation(position))
            {
                parent = _panelFactory.MakeNewLayoutPanel();
                parent.SetOrientation(position);
                ChangeParent(parent, panel);
                _events.RaiseParentChanged(parent, panel);
            }
            var childPanel = _panelFactory.MakeNewContentPanel();
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
            if (panel is not ContentPanel contentPanel) throw new ArgumentException($"{panelName} does not contain tabs.");
            var tab = contentPanel.TabCollection.Add(content);
            _events.RaiseTabAdded(contentPanel, tab);

            return (contentPanel, tab);
        }
    }

    private void ChangeParent(LayoutPanel parent, Panel child)
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
