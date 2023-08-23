using System.Collections.Generic;
using System.ComponentModel;

namespace WindowsEnvironment.Model;

public enum PanelOrientation { Unspecified, ByRows, ByCols }

public enum PanelState { Set, Flex }

public interface IPanel : INotifyPropertyChanged
{
    string Name { get; }

    IPanel? Parent { get; }

    PanelOrientation Orientation { get; }

    IReadOnlyList<IPanel> Children { get; }

    IReadOnlyCollection<IContentTab> Tabs { get; }

    string? SelectedTabName { get; set; }

    double? Size { get; set; }

    bool IsMain { get; }

    bool AllowTabs { get; }

    PanelState State { get; }
}
