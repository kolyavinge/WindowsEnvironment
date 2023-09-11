using System.Collections.Generic;
using System.Linq;

namespace WindowsEnvironment.Model;

internal class LayoutPanel : Panel, ILayoutPanel
{
    #region ILayoutPanel
    IReadOnlyList<IPanel> ILayoutPanel.Children => Children;
    #endregion

    public PanelOrientation Orientation { get; private set; }

    public List<Panel> Children { get; }

    public LayoutPanel(string name) : base(name)
    {
        Orientation = PanelOrientation.Unspecified;
        Children = new List<Panel>();
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
        if (!Children.Any()) return Enumerable.Empty<Panel>();
        var result = new List<Panel>();
        foreach (var child in Children)
        {
            result.Add(child);
            if (child is LayoutPanel layoutPanel)
            {
                result.AddRange(layoutPanel.GetAllChildren());
            }
        }

        return result;
    }
}
