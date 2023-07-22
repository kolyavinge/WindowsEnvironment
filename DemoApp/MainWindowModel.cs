using WindowsEnvironment;
using WindowsEnvironment.Model;

namespace DemoApp;

public class MainWindowModel
{
    public IFlexWindowsEnvironment Model { get; }

    public MainWindowModel()
    {
        Model = FlexWindowsEnvironmentFactory.Make();
        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new TestTabContent());
        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new TestTabContent());
        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new TestTabContent());
        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Right, new TestTabContent());
        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Right, new TestTabContent());
        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Bottom, new TestTabContent());
        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Bottom, new TestTabContent());
    }
}
