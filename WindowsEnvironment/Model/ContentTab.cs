namespace WindowsEnvironment.Model;

public interface IContentTab
{
    string Name { get; }

    Content Content { get; }
}

internal record ContentTab(string Name, Content Content) : IContentTab
{
    public static readonly string RootName = "tab_0";
}
