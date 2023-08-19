using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public interface IFlexWindowsEnvironment
{
    IEvents Events { get; }

    IPanel RootPanel { get; }

    IEnumerable<IPanel> AllPanels { get; }

    IPanel GetPanelByName(string name);

    (IPanel, IContentTab) GetTabByName(string name);

    (IPanel, IContentTab) GetTabById(object id);

    int GetChildPanelIndex(string parentPanelName, string childPanelName);

    (IPanel, IContentTab) SetPanelPosition(string panelName, PanelPosition position, Content configuration);

    void SelectTab(string tabName);

    void RemoveTab(string tabName, RemoveTabMode mode);

    double? GetPanelSize(string panelName);

    void SetPanelSize(string panelName, double? size);

    IFlexWindowsEnvironmentReader MakeReader();
}
