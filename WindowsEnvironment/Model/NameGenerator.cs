namespace WindowsEnvironment.Model;

internal interface INameGenerator
{
    string GetPanelName();
    string GetContentTabName();
}

internal class NameGenerator : INameGenerator
{
    private int _panelsCounter;
    private int _tabsCounter;

    public string GetPanelName()
    {
        return $"panel_{_panelsCounter++}";
    }

    public string GetContentTabName()
    {
        return $"tab_{_tabsCounter++}";
    }
}
