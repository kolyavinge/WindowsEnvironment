using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public enum PanelOrientation { Unspecified, ByRows, ByCols }

public interface ILayoutPanel : IPanel
{
    PanelOrientation Orientation { get; }

    IReadOnlyList<IPanel> Children { get; }
}
