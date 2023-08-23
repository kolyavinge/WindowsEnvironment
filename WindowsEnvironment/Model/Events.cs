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
    void RaiseTabRemoved(RemovedPanelInfo? removedPanel, Panel tabPanel, ContentTab tab, RemoveTabMode mode);
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

    public void RaiseTabRemoved(RemovedPanelInfo? removedPanel, Panel tabPanel, ContentTab tab, RemoveTabMode mode)
    {
        TabRemoved?.Invoke(this, new(removedPanel, tabPanel, tab, mode));
    }

    public void RaiseTabSelected(Panel tabPanel, ContentTab tab)
    {
        TabActivated?.Invoke(this, new(tabPanel, tab));
    }
}

public class PanelAddedEventArgs(IPanel parent, IPanel childPanel, IContentTab tab) : EventArgs
{
    public IPanel ParentPanel { get; } = parent;
    public IPanel ChildPanel { get; } = childPanel;
    public IContentTab Tab { get; } = tab;
}

public class ParentChangedEventArgs(IPanel parentPanel, IPanel childPanel) : EventArgs
{
    public IPanel ParentPanel { get; } = parentPanel;
    public IPanel ChildPanel { get; } = childPanel;
}

public class TabAddedEventArgs(IPanel parentPanel, IContentTab tab) : EventArgs
{
    public IPanel ParentPanel { get; } = parentPanel;
    public IContentTab Tab { get; } = tab;
}

public record RemovedPanelInfo(IPanel Parent, IPanel? Removed);

public class TabRemovedEventArgs(RemovedPanelInfo? removedPanel, IPanel tabPanel, IContentTab tab, RemoveTabMode mode) : EventArgs
{
    public RemovedPanelInfo? RemovedPanel { get; } = removedPanel;
    public IPanel TabPanel { get; } = tabPanel;
    public IContentTab Tab { get; } = tab;
    public RemoveTabMode Mode { get; } = mode;
}

public class TabSelectedEventArgs(IPanel tabPanel, IContentTab tab) : EventArgs
{
    public IPanel TabPanel { get; } = tabPanel;
    public IContentTab Tab { get; } = tab;
}
