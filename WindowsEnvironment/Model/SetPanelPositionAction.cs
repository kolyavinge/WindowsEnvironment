using System.Linq;

namespace WindowsEnvironment.Model;

internal interface ISetPanelPositionAction
{
    (Panel, ContentTab) SetPanelPosition(string panelName, PanelPosition position, Content configuration);
}

internal class SetPanelPositionAction : ISetPanelPositionAction
{
    private readonly IPanelCollectionInternal _panels;
    private readonly IPanelFactory _panelFactory;
    private readonly IEventsInternal _events;

    public SetPanelPositionAction(
        IPanelCollectionInternal panels,
        IPanelFactory panelFactory,
        IEventsInternal events)
    {
        _panels = panels;
        _panelFactory = panelFactory;
        _events = events;
    }

    public (Panel, ContentTab) SetPanelPosition(string panelName, PanelPosition position, Content content)
    {
        var panel = _panels.GetPanelByName(panelName);
        ContentTab tab;
        if (position != PanelPosition.Middle)
        {
            if (panel.IsMain || panel.Tabs.Any())
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
            childPanel.Parent = panel;
            tab = childPanel.Tabs.Add(content);
            if (position == PanelPosition.Left)
            {
                panel.Children.AddBegin(childPanel);
            }
            else if (position == PanelPosition.Right)
            {
                panel.Children.AddEnd(childPanel);
            }
            else if (position == PanelPosition.Top)
            {
                panel.Children.AddBegin(childPanel);
            }
            else if (position == PanelPosition.Bottom)
            {
                panel.Children.AddEnd(childPanel);
            }
            _events.RaisePanelAdded(panel, childPanel, tab);
        }
        else
        {
            if (!panel.AllowTabs) throw new ArgumentException($"{panelName} does not contain tabs.");
            tab = panel.Tabs.Add(content);
            _events.RaiseTabAdded(panel, tab);
        }

        return (panel, tab);
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
        var oldParent = child.Parent;
        if (oldParent != null)
        {
            oldParent.Children.Remove(child);
            oldParent.Children.AddEnd(parent);
        }
        else
        {
            _panels.SetRoot(parent);
        }
        child.Parent = parent;
        parent.Parent = oldParent;
        parent.Children.AddEnd(child);
    }
}
