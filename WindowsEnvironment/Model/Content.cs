namespace WindowsEnvironment.Model;

public record Header
{
    public object SourceObject { get; set; } = new object();

    public string PropertyName { get; set; } = "";
}

public record Content
{
    public Header Header { get; set; } = new Header();

    public object View { get; set; } = new object();

    public Action? CloseCallback { get; set; }
}
