namespace WindowsEnvironment.Model;

public class Header
{
    public object SourceObject { get; set; } = new object();

    public string PropertyName { get; set; } = "";
}

public class Content
{
    public Header Header { get; set; } = new Header();

    public object View { get; set; } = new object();

    public Action? CloseCallback { get; set; }
}
