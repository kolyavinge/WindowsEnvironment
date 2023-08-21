using System.Collections.Generic;
using System.ComponentModel;

namespace WindowsEnvironment.Model;

public interface IPanel
{
    event PropertyChangedEventHandler? PropertyChanged;

    string Name { get; }

    IPanel? Parent { get; }

    SplitOrientation Orientation { get; }

    IReadOnlyList<IPanel> Children { get; }

    IReadOnlyCollection<IContentTab> Tabs { get; }

    string? SelectedTabName { get; set; }

    double? Size { get; set; }

    bool IsMain { get; }

    bool AllowTabs { get; }
}
