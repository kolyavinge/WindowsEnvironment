using WindowsEnvironment.Utils;

namespace WindowsEnvironment.Model;

internal interface ISetPanelPositionAction
{
    ContentTab SetPanelPosition(string panelName, PanelPosition position, Content content);
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

    public ContentTab SetPanelPosition(string panelName, PanelPosition position, Content content)
    {
        _panels.RemoveFlexPanelTabById(content.Id);
        var panel = _panels.GetPanelByName(panelName);
        if (position != PanelPosition.Middle)
        {
            var parent = panel.Parent;
            if (parent == null || !parent.IsSuitableOrientation(position))
            {
                parent = _panelFactory.MakeNewLayoutPanel();
                parent.SetOrientation(position);
                ChangeParent(parent, panel);
                _events.RaiseParentChanged(parent, panel);
            }
            var childPanel = _panelFactory.MakeNewContentPanel();
            childPanel.Parent = parent;
            var childPanelIndex = parent.Children.IndexOf(panel);
            if (position is PanelPosition.Right or PanelPosition.Bottom) childPanelIndex++;
            parent.Children.Insert(childPanelIndex, childPanel);
            var tab = childPanel.Tab.Add(content);
            _events.RaisePanelAdded(parent, childPanel, tab);

            return tab;
        }
        else
        {
            if (panel is not ContentPanel contentPanel) throw new ArgumentException($"{panelName} does not contain tabs.");
            var tab = contentPanel.Tab.Add(content);
            _events.RaiseTabAdded(contentPanel, tab);

            return tab;
        }
    }

    private void ChangeParent(LayoutPanel parent, Panel child)
    {
        var oldParent = child.Parent;
        if (oldParent != null)
        {
            oldParent.Children.Replace(child, parent);
        }
        else
        {
            _panels.SetRoot(parent);
        }
        child.Parent = parent;
        parent.Parent = oldParent;
        parent.Children.Add(child);
    }
}
