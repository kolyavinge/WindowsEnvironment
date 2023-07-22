using System.Collections.Generic;
using System.Linq;

namespace WindowsEnvironment.Model;

public class Panel
{
    public static readonly string MainPanelName = "panel_0";

    public string Name { get; }

    public Panel? Parent { get; internal set; }

    public SplitOrientation Orientation { get; private set; }

    public PanelChildrenCollection Children { get; private set; }

    public ContentTabCollection Tabs { get; private set; }

    public bool IsMain => Name == MainPanelName;

    public bool IsRoot => Parent == null;

    public bool AllowTabs => !Children.Any();

    public Panel(string name, ContentTabCollection tabs)
    {
        Name = name;
        Orientation = SplitOrientation.Unspecified;
        Children = new PanelChildrenCollection();
        Tabs = tabs;
    }

    internal void SetOrientation(PanelPosition position)
    {
        if (position is PanelPosition.Middle)
        {
            throw new ArgumentException($"Position must be {PanelPosition.Left}, {PanelPosition.Right}, {PanelPosition.Top} or {PanelPosition.Bottom}.");
        }

        Orientation = position is PanelPosition.Left or PanelPosition.Right
            ? SplitOrientation.ByCols
            : SplitOrientation.ByRows;
    }

    internal IEnumerable<Panel> GetAllChildren()
    {
        if (!Children.Any()) return Enumerable.Empty<Panel>();
        var result = new List<Panel>();
        foreach (var child in Children)
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
