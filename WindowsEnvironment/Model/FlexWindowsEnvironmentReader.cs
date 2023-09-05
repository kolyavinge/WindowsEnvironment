namespace WindowsEnvironment.Model;

public class PanelReadEventArgs : EventArgs
{
    public IPanel Panel { get; }
    internal PanelReadEventArgs(IPanel panel)
    {
        Panel = panel;
    }
}

public class TabReadEventArgs : EventArgs
{
    public IContentPanel Panel { get; }
    public IContentTab Tab { get; }
    internal TabReadEventArgs(IContentPanel panel, IContentTab tab)
    {
        Panel = panel;
        Tab = tab;
    }
}

public interface IFlexWindowsEnvironmentReader
{
    event EventHandler<PanelReadEventArgs>? BeginPanelRead;
    event EventHandler<TabReadEventArgs>? TabRead;
    event EventHandler<PanelReadEventArgs>? EndPanelRead;
    void Read();
}

internal class FlexWindowsEnvironmentReader : IFlexWindowsEnvironmentReader
{
    private readonly IFlexWindowsEnvironment _model;

    public event EventHandler<PanelReadEventArgs>? BeginPanelRead;
    public event EventHandler<TabReadEventArgs>? TabRead;
    public event EventHandler<PanelReadEventArgs>? EndPanelRead;

    public FlexWindowsEnvironmentReader(IFlexWindowsEnvironment model)
    {
        _model = model;
    }

    public void Read()
    {
        Read(_model.RootPanel);
    }

    private void Read(IPanel panel)
    {
        BeginPanelRead?.Invoke(this, new(panel));

        if (panel is ILayoutPanel layoutPanel)
        {
            foreach (var child in layoutPanel.Children)
            {
                Read(child);
            }
        }

        if (panel is IContentPanel contentPanel)
        {
            foreach (var tab in contentPanel.Tabs)
            {
                TabRead?.Invoke(this, new(contentPanel, tab));
            }
        }

        EndPanelRead?.Invoke(this, new(panel));
    }
}
