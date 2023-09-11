using System.ComponentModel;

namespace WindowsEnvironment.Model;

public static class MainPanel
{
    public static readonly string Name = "panel_0";
}

internal abstract class Panel : IPanel
{
    #region IPanel
    ILayoutPanel? IPanel.Parent => Parent;
    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; }

    public LayoutPanel? Parent { get; set; }

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
