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
        int id = 1;
        _statusText = "";
        Model = FlexWindowsEnvironmentFactory.Make();

        Model.SetPanelPosition(MainPanel.Name, PanelPosition.Middle, new(id++)
        {
            Header = new()
            {
                SourceObject = new UpdateableTabContentHeader(),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Updateable closed"
        });

        var tab = Model.SetPanelPosition(MainPanel.Name, PanelPosition.Middle, new(id++)
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 1"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 1 closed"
        });
        Model.SelectTab(tab.Name);

        Model.SetPanelPosition(MainPanel.Name, PanelPosition.Middle, new(id++)
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 2"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 2 closed"
        });

        tab = Model.SetPanelPosition(MainPanel.Name, PanelPosition.Right, new(id++)
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 3"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 3 closed"
        });
        Model.SetPanelSize(tab.Parent.Name, 100);

        Model.SetPanelPosition(MainPanel.Name, PanelPosition.Right, new(id++)
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 4"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 4 closed"
        });

        Model.SetPanelPosition(MainPanel.Name, PanelPosition.Bottom, new(id++)
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 5"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 5 closed"
        });

        tab = Model.SetPanelPosition(MainPanel.Name, PanelPosition.Bottom, new(id++)
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader("Header 6"),
                PropertyName = "Text"
            },
            View = new TestTabContent(),
            CloseCallback = () => StatusText = "Header 6 closed"
        });
        Model.SetPanelSize(tab.Parent.Name, 100);
    }
}
