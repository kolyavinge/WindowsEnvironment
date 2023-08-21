using System.Linq;

namespace WindowsEnvironment.Model;

internal interface ISetPanelPositionAction
{
    (IPanel, IContentTab) SetPanelPosition(string panelName, PanelPosition position, Content configuration);
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

    public (IPanel, IContentTab) SetPanelPosition(string panelName, PanelPosition position, Content content)
    {
        _panels.RemoveFlexPanelTabById(content.Id);
        var panel = _panels.GetPanelByName(panelName);
        if (position != PanelPosition.Middle)
        {
            if (panel.IsMain || panel.ContentTabCollection.Any())
            {
                var newParent = _panelFactory.MakeNew();
                ChangeParent(newParent, panel);
                _events.RaiseParentChanged(newParent, panel);
                panel = newParent;
            }
            if (panel.Orientation == SplitOrientation.Unspecified)
            {
                panel.SetOrientation(position);
            }
            else
            {
                ChangeSplitOrientationIfNeeded(ref panel, position);
            }
            var childPanel = _panelFactory.MakeNew();
            childPanel.ParentPanel = panel;
            var tab = childPanel.ContentTabCollection.Add(content);
            if (position == PanelPosition.Left)
            {
                panel.ChildrenCollection.AddBegin(childPanel);
            }
            else if (position == PanelPosition.Right)
            {
                panel.ChildrenCollection.AddEnd(childPanel);
            }
            else if (position == PanelPosition.Top)
            {
                panel.ChildrenCollection.AddBegin(childPanel);
            }
            else if (position == PanelPosition.Bottom)
            {
                panel.ChildrenCollection.AddEnd(childPanel);
            }
            _events.RaisePanelAdded(panel, childPanel, tab);

            return (childPanel, tab);
        }
        else
        {
            if (!panel.AllowTabs) throw new ArgumentException($"{panelName} does not contain tabs.");
            var tab = panel.ContentTabCollection.Add(content);
            _events.RaiseTabAdded(panel, tab);

            return (panel, tab);
        }
    }

    private void ChangeSplitOrientationIfNeeded(ref Panel panel, PanelPosition position)
    {
        if (panel.Orientation == SplitOrientation.Unspecified) return;
        if (panel.Orientation == SplitOrientation.ByCols && (position is PanelPosition.Left or PanelPosition.Right)) return;
        if (panel.Orientation == SplitOrientation.ByRows && (position is PanelPosition.Top or PanelPosition.Bottom)) return;
        var newParent = _panelFactory.MakeNew();
        ChangeParent(newParent, panel);
        newParent.SetOrientation(position);
        _events.RaiseParentChanged(newParent, panel);
        panel = newParent;
    }

    private void ChangeParent(Panel parent, Panel child)
    {
        var oldParent = child.ParentPanel;
        if (oldParent != null)
        {
            oldParent.ChildrenCollection.Remove(child);
            oldParent.ChildrenCollection.AddEnd(parent);
        }
        else
        {
            _panels.SetRoot(parent);
        }
        child.ParentPanel = parent;
        parent.ParentPanel = oldParent;
        parent.ChildrenCollection.AddEnd(child);
    }
}
