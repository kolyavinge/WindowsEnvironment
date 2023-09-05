using System.Collections.Generic;

namespace WindowsEnvironment.Model;

internal class ContentPanel : Panel, IContentPanel
{
    public IReadOnlyCollection<IContentTab> Tabs => TabCollection;

    public ContentTabCollection TabCollection { get; }

    private string? _selectedTabName;
    public string? SelectedTabName
    {
        get => _selectedTabName;
        set { _selectedTabName = value; RaisePropertyChanged(nameof(SelectedTabName)); }
    }

    private double? _size;
    public double? Size
    {
        get { return _size; }
        set { _size = value; RaisePropertyChanged(nameof(Size)); }
    }

    public bool IsMain => Name == MainPanel.Name;

    public PanelState State { get; internal set; }

    public ContentPanel(string name, ContentTabCollection tabs) : base(name)
    {
        TabCollection = tabs;
        State = PanelState.Set;
    }
}
