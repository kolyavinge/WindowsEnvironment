using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WindowsEnvironment.Model;

public static class MainPanel
{
    public static readonly string Name = "panel_0";
}

internal class Panel : IPanel, INotifyPropertyChanged
{
    private string? _selectedTabName;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; }

    public IPanel? Parent => ParentPanel;

    public Panel? ParentPanel { get; internal set; }

    public SplitOrientation Orientation { get; private set; }

    public PanelChildrenCollection ChildrenCollection { get; }

    public IReadOnlyList<IPanel> Children => ChildrenCollection;

    public IReadOnlyCollection<IContentTab> Tabs => ContentTabCollection;

    public ContentTabCollection ContentTabCollection { get; }

    public string? SelectedTabName
    {
        get => _selectedTabName;
        set { _selectedTabName = value; PropertyChanged?.Invoke(this, new(nameof(SelectedTabName))); }
    }

    private double? _size;
    public double? Size
    {
        get { return _size; }
        set { _size = value; PropertyChanged?.Invoke(this, new(nameof(Size))); }
    }

    public bool IsMain => Name == MainPanel.Name;

    public bool AllowTabs => !ChildrenCollection.Any();

    public PanelState State { get; internal set; }

    public Panel(string name, ContentTabCollection tabs)
    {
        Name = name;
        Orientation = SplitOrientation.Unspecified;
        ChildrenCollection = new PanelChildrenCollection();
        ContentTabCollection = tabs;
        State = PanelState.Set;
    }

    public void SetOrientation(PanelPosition position)
    {
        if (position is PanelPosition.Middle)
        {
            throw new ArgumentException($"Position must be {PanelPosition.Left}, {PanelPosition.Right}, {PanelPosition.Top} or {PanelPosition.Bottom}.");
        }

        Orientation = position is PanelPosition.Left or PanelPosition.Right
            ? SplitOrientation.ByCols
            : SplitOrientation.ByRows;
    }

    public IEnumerable<Panel> GetAllChildren()
    {
        if (!ChildrenCollection.Any()) return Enumerable.Empty<Panel>();
        var result = new List<Panel>();
        foreach (var child in ChildrenCollection)
        {
            result.Add(child);
            result.AddRange(child.GetAllChildren());
        }

        return result;
    }

    public override string ToString()
    {
        return Name;
    }
}
