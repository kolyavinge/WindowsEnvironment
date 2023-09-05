using WindowsEnvironment.Model;

namespace WindowsEnvironment.View;

internal class MasterGridInitializer
{
    private readonly IFlexWindowsEnvironmentReader _reader;
    private readonly MasterGrid _masterGrid;

    public MasterGridInitializer(IFlexWindowsEnvironmentReader reader, MasterGrid masterGrid)
    {
        _reader = reader;
        _masterGrid = masterGrid;
    }

    public void Init()
    {
        _reader.BeginPanelRead += (s, e) =>
        {
            _masterGrid.AddPanel(e.Panel.Parent, e.Panel);
        };

        _reader.TabRead += (s, e) =>
        {
            _masterGrid.AddTab(e.Panel, e.Tab);
        };

        _reader.EndPanelRead += (s, e) =>
        {
            if (e.Panel is ILayoutPanel layoutPanel)
            {
                _masterGrid.MakeSplitters(layoutPanel);
                _masterGrid.SetPanelRowsCols(layoutPanel);
                _masterGrid.SetSplittersRowsCols(layoutPanel);
            }
        };

        _reader.Read();
    }
}
