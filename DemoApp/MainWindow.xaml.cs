using System;
using System.Windows;
using WindowsEnvironment.Model;

namespace DemoApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowModel();
    }

    private void AddNewTabClick(object sender, RoutedEventArgs e)
    {
        AddNewTab();
    }

    private void AddNewTab()
    {
        flex.Model.SetPanelPosition(Panel.MainPanelName, PanelPosition.Middle, new()
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader($"Header {DateTime.Now.Second}"),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });
    }
}
