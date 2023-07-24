using WindowsEnvironment;
using WindowsEnvironment.Model;

namespace DemoApp;

public class MainWindowModel
{
    public IFlexWindowsEnvironment Model { get; }

    public MainWindowModel()
    {
        Model = FlexWindowsEnvironmentFactory.Make();

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new()
        {
            Header = new()
            {
                SourceObject = new UpdateableTabContentHeader(),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 2"),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 3"),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Right, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 4"),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Right, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 5"),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Bottom, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 6"),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Bottom, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 7"),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });
    }
}
