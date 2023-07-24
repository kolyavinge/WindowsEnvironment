using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace DemoApp;

public class TestTabContentHeader
{
    public string Text { get; }

    public TestTabContentHeader(string text)
    {
        Text = text;
    }
}

public class UpdateableTabContentHeader : INotifyPropertyChanged
{
    private readonly Timer _timer;
    private string _text;
    private int _count;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            PropertyChanged?.Invoke(this, new("Text"));
        }
    }

    public UpdateableTabContentHeader()
    {
        _text = "";
        _count = 1;
        _timer = new Timer((s) => { Text = $"Updateable {_count}"; _count++; }, null, 0, 5000);
    }
}

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
