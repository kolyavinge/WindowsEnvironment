using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public enum PanelState { Set, Flex }

public interface IContentPanel : IPanel
{
    IReadOnlyCollection<IContentTab> Tabs { get; }

    string? SelectedTabName { get; set; }

    double? Size { get; set; }

    bool IsMain { get; }

    PanelState State { get; }
}
