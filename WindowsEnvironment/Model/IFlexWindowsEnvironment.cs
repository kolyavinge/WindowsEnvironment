using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public interface IFlexWindowsEnvironment
{
    IEvents Events { get; }

    IPanel RootPanel { get; }

    IEnumerable<IPanel> AllPanels { get; }

    IPanel GetPanelByName(string name);

    (IContentPanel, IContentTab) GetTabByName(string name);

    (IContentPanel, IContentTab) GetTabById(object id);

    int GetChildPanelIndex(string parentPanelName, string childPanelName);

    (IContentPanel, IContentTab) SetPanelPosition(string panelName, PanelPosition position, Content content);

    void SelectTab(string tabName);

    void RemoveTab(string tabName, RemoveTabMode mode);

    double? GetPanelSize(string panelName);

    void SetPanelSize(string panelName, double? size);

    IFlexWindowsEnvironmentReader MakeReader();
}
