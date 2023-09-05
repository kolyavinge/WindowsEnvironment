using System.ComponentModel;

namespace WindowsEnvironment.Model;

public static class MainPanel
{
    public static readonly string Name = "panel_0";
}

internal abstract class Panel : IPanel
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; }

    public ILayoutPanel? Parent => ParentPanel;

    public LayoutPanel? ParentPanel { get; internal set; }

    public Panel(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    protected void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}
