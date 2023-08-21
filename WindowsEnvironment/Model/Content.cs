namespace WindowsEnvironment.Model;

public record Header
{
    public object SourceObject { get; set; } = new();

    public string PropertyName { get; set; } = "";
}

public record Content(object Id)
{
    public Header Header { get; set; } = new();

    public object View { get; set; } = new();

    public Action? CloseCallback { get; set; }
}
