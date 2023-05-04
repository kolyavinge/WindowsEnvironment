using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsEnvironment.Model;

namespace DemoApp;

public partial class MainWindow : Window
{
    private readonly Random _rand = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowModel();
    }

    private void OnModelInitialized(object sender, EventArgs e)
    {
        AddNewTab();
        AddNewTab();
        AddNewTab();
    }

    private void AddNewTabClick(object sender, RoutedEventArgs e)
    {
        AddNewTab();
    }

    private void AddNewTab()
    {
        var r = (byte)(_rand.Next() % 255);
        var g = (byte)(_rand.Next() % 255);
        var b = (byte)(_rand.Next() % 255);

        flex.Model.SetPanelPosition(
            WindowsEnvironment.Model.Panel.MainPanelName, PanelPosition.Middle, new TextBlock { Background = new SolidColorBrush(new() { R = r, G = g, B = b, A = 255 }) });
    }
}
