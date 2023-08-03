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
    public IPanel ParentPanel { get; }
    public IPanel ChildPanel { get; }
    public IContentTab Tab { get; }

    public PanelAddedEventArgs(IPanel parent, IPanel childPanel, IContentTab tab)
    {
        ParentPanel = parent;
        ChildPanel = childPanel;
        Tab = tab;
    }
}

public class ParentChangedEventArgs : EventArgs
{
    public IPanel ParentPanel { get; }
    public IPanel ChildPanel { get; }

    public ParentChangedEventArgs(IPanel parentPanel, IPanel childPanel)
    {
        ParentPanel = parentPanel;
        ChildPanel = childPanel;
    }
}

public class TabAddedEventArgs : EventArgs
{
    public IPanel ParentPanel { get; }
    public IContentTab Tab { get; }

    public TabAddedEventArgs(IPanel parentPanel, IContentTab tab)
    {
        ParentPanel = parentPanel;
        Tab = tab;
    }
}

public record RemovedPanel(IPanel Parent, IPanel Removed);

public class TabRemovedEventArgs : EventArgs
{
    public RemovedPanel? RemovedPanel { get; }
    public IPanel TabPanel { get; }
    public IContentTab Tab { get; }
    public RemoveTabMode Mode { get; }

    public TabRemovedEventArgs(RemovedPanel? removedPanel, IPanel tabPanel, IContentTab tab, RemoveTabMode mode)
    {
        RemovedPanel = removedPanel;
        TabPanel = tabPanel;
        Tab = tab;
        Mode = mode;
    }
}

public class TabSelectedEventArgs : EventArgs
{
    public IPanel TabPanel { get; }
    public IContentTab Tab { get; }

    public TabSelectedEventArgs(IPanel tabPanel, IContentTab tab)
    {
        TabPanel = tabPanel;
        Tab = tab;
    }
}
