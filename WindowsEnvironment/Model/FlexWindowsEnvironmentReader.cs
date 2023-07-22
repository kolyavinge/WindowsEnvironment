namespace WindowsEnvironment.Model;

public class PanelReadEventArgs : EventArgs
{
    public Panel Panel { get; }
    public PanelReadEventArgs(Panel panel)
    {
        Panel = panel;
    }
}

public class TabReadEventArgs : EventArgs
{
    public Panel Panel { get; }
    public ContentTab Tab { get; }
    public TabReadEventArgs(Panel panel, ContentTab tab)
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

    private void Read(Panel panel)
    {
        BeginPanelRead?.Invoke(this, new(panel));
        foreach (var child in panel.Children)
        {
            Read(child);
        }
        foreach (var tab in panel.Tabs)
        {
            TabRead?.Invoke(this, new(panel, tab));
        }
        EndPanelRead?.Invoke(this, new(panel));
    }
}
