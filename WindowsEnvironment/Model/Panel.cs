using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WindowsEnvironment.Model;

public static class MainPanel
{
    public static readonly string Name = "panel_0";
}

internal class Panel : IPanel
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; }

    public IPanel? Parent => ParentPanel;

    public Panel? ParentPanel { get; internal set; }

    public PanelOrientation Orientation { get; private set; }

    public IReadOnlyList<IPanel> Children => ChildrenList;

    public List<Panel> ChildrenList { get; }

    public IReadOnlyCollection<IContentTab> Tabs => TabCollection;

    public ContentTabCollection TabCollection { get; }

    private string? _selectedTabName;
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

    public bool AllowTabs => !ChildrenList.Any();

    public PanelState State { get; internal set; }

    public Panel(string name, ContentTabCollection tabs)
    {
        Name = name;
        Orientation = PanelOrientation.Unspecified;
        ChildrenList = new List<Panel>();
        TabCollection = tabs;
        State = PanelState.Set;
    }

    public void SetOrientation(PanelPosition position)
    {
        if (position is PanelPosition.Middle)
        {
            throw new ArgumentException($"Position must be {PanelPosition.Left}, {PanelPosition.Right}, {PanelPosition.Top} or {PanelPosition.Bottom}.");
        }

        Orientation = position is PanelPosition.Left or PanelPosition.Right
            ? PanelOrientation.ByCols
            : PanelOrientation.ByRows;
    }

    public bool IsSuitableOrientation(PanelPosition position)
    {
        if (position is PanelPosition.Middle)
        {
            throw new ArgumentException($"Position must be {PanelPosition.Left}, {PanelPosition.Right}, {PanelPosition.Top} or {PanelPosition.Bottom}.");
        }

        if (Orientation == PanelOrientation.Unspecified) return true;
        if (Orientation == PanelOrientation.ByCols && (position is PanelPosition.Left or PanelPosition.Right)) return true;
        if (Orientation == PanelOrientation.ByRows && (position is PanelPosition.Top or PanelPosition.Bottom)) return true;

        return false;
    }

    public IEnumerable<Panel> GetAllChildren()
    {
        if (!ChildrenList.Any()) return Enumerable.Empty<Panel>();
        var result = new List<Panel>();
        foreach (var child in ChildrenList)
        {
            result.Add(child);
            result.AddRange(child.GetAllChildren());
        }

        return result;
    }

    public override string ToString() => Name;
}
