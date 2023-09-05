using System;
using System.Linq;
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
        flex.Model.SetPanelPosition(MainPanel.Name, PanelPosition.Middle, new(DateTime.Now.Ticks)
        {
            Header = new()
            {
                SourceObject = new TestTabContentHeader($"Header {DateTime.Now.Second}"),
                PropertyName = "Text"
            },
            View = new TestTabContent()
        });
    }

    private void SelectTabClick(object sender, RoutedEventArgs e)
    {
        flex.Model.SelectTab("tab_1");
    }

    private void CloseAllClick(object sender, RoutedEventArgs e)
    {
        foreach (var panel in flex.Model.AllPanels.OfType<IContentPanel>().ToList())
        {
            foreach (var tab in panel.Tabs.ToList())
            {
                flex.Model.RemoveTab(tab.Name, RemoveTabMode.Close);
            }
        }
    }
}
