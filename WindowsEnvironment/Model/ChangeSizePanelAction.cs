﻿namespace WindowsEnvironment.Model;

internal interface IChangeSizePanelAction
{
    double? GetPanelSize(string panelName);
    void SetPanelSize(string panelName, double? size);
}

internal class ChangeSizePanelAction : IChangeSizePanelAction
{
    private readonly IPanelCollection _panels;

    public ChangeSizePanelAction(IPanelCollection panels)
    {
        _panels = panels;
    }

    public double? GetPanelSize(string panelName)
    {
        var panel = (ContentPanel)_panels.GetPanelByName(panelName); // check
        return panel.Size;
    }

    public void SetPanelSize(string panelName, double? size)
    {
        var panel = (ContentPanel)_panels.GetPanelByName(panelName);
        panel.Size = size;
    }
}
