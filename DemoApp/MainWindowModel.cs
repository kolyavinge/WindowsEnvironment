using System.ComponentModel;
using WindowsEnvironment;
using WindowsEnvironment.Model;

namespace DemoApp;

public class MainWindowModel : INotifyPropertyChanged
{
    private string _statusText;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IFlexWindowsEnvironment Model { get; }

    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            PropertyChanged?.Invoke(this, new("StatusText"));
        }
    }

    public MainWindowModel()
    {
        _statusText = "";
        Model = FlexWindowsEnvironmentFactory.Make();

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new()
        {
            Header = new()
            {
                SourceObject = new UpdateableTabContentHeader(),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Updateable closed"
        });

        var (panel, tab) = Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 1"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 1 closed"
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 2"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 2 closed"
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Right, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 3"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 3 closed"
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Right, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 4"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 4 closed"
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Bottom, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 5"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 5 closed"
        });

        Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Bottom, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 6"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 6 closed"
        });

        Model.SelectTab(panel.Name, tab.Name);
    }
}
