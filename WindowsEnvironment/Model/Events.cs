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
    void RaisePanelAdded(LayoutPanel parent, ContentPanel childPanel, ContentTab tab);
    void RaiseParentChanged(LayoutPanel parentPanel, Panel childPanel);
    void RaiseTabAdded(ContentPanel parentPanel, ContentTab tab);
    void RaiseTabRemoved(RemovedPanelInfo? removedPanel, ContentPanel tabPanel, ContentTab tab, RemoveTabMode mode);
    void RaiseTabSelected(ContentPanel tabPanel, ContentTab tab);
}

internal class Events : IEventsInternal
{
    public event EventHandler<PanelAddedEventArgs>? PanelAdded;
    public event EventHandler<ParentChangedEventArgs>? ParentChanged;
    public event EventHandler<TabAddedEventArgs>? TabAdded;
    public event EventHandler<TabRemovedEventArgs>? TabRemoved;
    public event EventHandler<TabSelectedEventArgs>? TabActivated;

    public void RaisePanelAdded(LayoutPanel parent, ContentPanel childPanel, ContentTab tab)
    {
        PanelAdded?.Invoke(this, new(parent, childPanel, tab));
    }

    public void RaiseParentChanged(LayoutPanel parentPanel, Panel childPanel)
    {
        ParentChanged?.Invoke(this, new(parentPanel, childPanel));
    }

    public void RaiseTabAdded(ContentPanel parentPanel, ContentTab tab)
    {
        TabAdded?.Invoke(this, new(parentPanel, tab));
    }

    public void RaiseTabRemoved(RemovedPanelInfo? removedPanel, ContentPanel tabPanel, ContentTab tab, RemoveTabMode mode)
    {
        TabRemoved?.Invoke(this, new(removedPanel, tabPanel, tab, mode));
    }

    public void RaiseTabSelected(ContentPanel tabPanel, ContentTab tab)
    {
        TabActivated?.Invoke(this, new(tabPanel, tab));
    }
}

public class PanelAddedEventArgs(ILayoutPanel parent, IContentPanel childPanel, IContentTab tab) : EventArgs
{
    public ILayoutPanel ParentPanel { get; } = parent;
    public IContentPanel ChildPanel { get; } = childPanel;
    public IContentTab Tab { get; } = tab;
}

public class ParentChangedEventArgs(ILayoutPanel parentPanel, IPanel childPanel) : EventArgs
{
    public ILayoutPanel ParentPanel { get; } = parentPanel;
    public IPanel ChildPanel { get; } = childPanel;
}

public class TabAddedEventArgs(IContentPanel parentPanel, IContentTab tab) : EventArgs
{
    public IContentPanel ParentPanel { get; } = parentPanel;
    public IContentTab Tab { get; } = tab;
}

public record RemovedPanelInfo(ILayoutPanel Parent, IPanel? Removed);

public class TabRemovedEventArgs(RemovedPanelInfo? removedPanel, IContentPanel tabPanel, IContentTab tab, RemoveTabMode mode) : EventArgs
{
    public RemovedPanelInfo? RemovedPanel { get; } = removedPanel;
    public IContentPanel TabPanel { get; } = tabPanel;
    public IContentTab Tab { get; } = tab;
    public RemoveTabMode Mode { get; } = mode;
}

public class TabSelectedEventArgs(IContentPanel tabPanel, IContentTab tab) : EventArgs
{
    public IContentPanel TabPanel { get; } = tabPanel;
    public IContentTab Tab { get; } = tab;
}
