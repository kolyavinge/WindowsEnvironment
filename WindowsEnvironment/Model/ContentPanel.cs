using System.Collections.Generic;

namespace WindowsEnvironment.Model;

internal class ContentPanel : Panel, IContentPanel
{
    #region IContentPanel
    IReadOnlyCollection<IContentTab> IContentPanel.Tabs => Tab;
    #endregion

    public ContentTabCollection Tab { get; }

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

    public PanelState State { get; set; }

    public ContentPanel(string name, ContentTabCollection tabs) : base(name)
    {
        Tab = tabs;
        Tab.ParentPanel = this;
        State = PanelState.Set;
    }
}
