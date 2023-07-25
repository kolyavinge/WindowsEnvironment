namespace WindowsEnvironment.Model;

public interface IEvents
{
    event EventHandler<PanelAddedEventArgs>? PanelAdded;
    event EventHandler<ParentChangedEventArgs>? ParentChanged;
    event EventHandler<TabAddedEventArgs>? TabAdded;
    event EventHandler<TabRemovedEventArgs>? TabRemoved;
    event EventHandler<TabSelectedEventArgs>? TabActivated;
}

internal interface IEventsInternal : IEvents
{
    void RaisePanelAdded(Panel parent, Panel childPanel, ContentTab tab);
    void RaiseParentChanged(Panel parentPanel, Panel childPanel);
    void RaiseTabAdded(Panel parentPanel, ContentTab tab);
    void RaiseTabRemoved(RemovedPanel? removedPanel, Panel tabPanel, ContentTab tab, RemoveTabMode mode);
    void RaiseTabSelected(Panel tabPanel, ContentTab tab);
}

internal class Events : IEventsInternal
{
    public event EventHandler<PanelAddedEventArgs>? PanelAdded;
    public event EventHandler<ParentChangedEventArgs>? ParentChanged;
    public event EventHandler<TabAddedEventArgs>? TabAdded;
    public event EventHandler<TabRemovedEventArgs>? TabRemoved;
    public event EventHandler<TabSelectedEventArgs>? TabActivated;

    public void RaisePanelAdded(Panel parent, Panel childPanel, ContentTab tab)
    {
        PanelAdded?.Invoke(this, new(parent, childPanel, tab));
    }

    public void RaiseParentChanged(Panel parentPanel, Panel childPanel)
    {
        ParentChanged?.Invoke(this, new(parentPanel, childPanel));
    }

    public void RaiseTabAdded(Panel parentPanel, ContentTab tab)
    {
        TabAdded?.Invoke(this, new(parentPanel, tab));
    }

    public void RaiseTabRemoved(RemovedPanel? removedPanel, Panel tabPanel, ContentTab tab, RemoveTabMode mode)
    {
        TabRemoved?.Invoke(this, new(removedPanel, tabPanel, tab, mode));
    }

    public void RaiseTabSelected(Panel tabPanel, ContentTab tab)
    {
        TabActivated?.Invoke(this, new(tabPanel, tab));
    }
}

public class PanelAddedEventArgs : EventArgs
{
    public Panel ParentPanel { get; }
    public Panel ChildPanel { get; }
    public ContentTab Tab { get; }

    public PanelAddedEventArgs(Panel parent, Panel childPanel, ContentTab tab)
    {
        ParentPanel = parent;
        ChildPanel = childPanel;
        Tab = tab;
    }
}

public class ParentChangedEventArgs : EventArgs
{
    public Panel ParentPanel { get; }
    public Panel ChildPanel { get; }

    public ParentChangedEventArgs(Panel parentPanel, Panel childPanel)
    {
        ParentPanel = parentPanel;
        ChildPanel = childPanel;
    }
}

public class TabAddedEventArgs : EventArgs
{
    public Panel ParentPanel { get; }
    public ContentTab Tab { get; }

    public TabAddedEventArgs(Panel parentPanel, ContentTab tab)
    {
        ParentPanel = parentPanel;
        Tab = tab;
    }
}

public record RemovedPanel(Panel Parent, Panel Removed);

public class TabRemovedEventArgs : EventArgs
{
    public RemovedPanel? RemovedPanel { get; }
    public Panel TabPanel { get; }
    public ContentTab Tab { get; }
    public RemoveTabMode Mode { get; }

    public TabRemovedEventArgs(RemovedPanel? removedPanel, Panel tabPanel, ContentTab tab, RemoveTabMode mode)
    {
        RemovedPanel = removedPanel;
        TabPanel = tabPanel;
        Tab = tab;
        Mode = mode;
    }
}

public class TabSelectedEventArgs : EventArgs
{
    public Panel TabPanel { get; }
    public ContentTab Tab { get; }

    public TabSelectedEventArgs(Panel tabPanel, ContentTab tab)
    {
        TabPanel = tabPanel;
        Tab = tab;
    }
}
