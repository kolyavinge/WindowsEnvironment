namespace WindowsEnvironment.Model;

internal interface IChangeSizePanelAction
{
    double? GetPanelSize(string panelName);
    void SetPanelSize(string panelName, double? size);
}

internal class ChangeSizePanelAction : IChangeSizePanelAction
{
    private readonly IPanelCollectionInternal _panels;

    public ChangeSizePanelAction(IPanelCollectionInternal panels)
    {
        _panels = panels;
    }

    public double? GetPanelSize(string panelName)
    {
        var panel = _panels.GetPanelByName(panelName);
        return panel.Size;
    }

    public void SetPanelSize(string panelName, double? size)
    {
        var panel = _panels.GetPanelByName(panelName);
        panel.Size = size;
    }
}
