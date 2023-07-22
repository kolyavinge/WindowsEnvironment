using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace DemoApp;

public class TestTabContent : TextBlock
{
    private static readonly Random _rand = new Random();

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        var r = (byte)(_rand.Next() % 255);
        var g = (byte)(_rand.Next() % 255);
        var b = (byte)(_rand.Next() % 255);

        Background = new SolidColorBrush(new() { R = r, G = g, B = b, A = 255 });
    }
}
