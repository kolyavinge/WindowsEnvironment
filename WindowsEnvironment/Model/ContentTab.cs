using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public interface IContentTab
{
    string Name { get; }

    Content Content { get; }

    IContentPanel Parent { get; }
}

internal class ContentTab : IContentTab, IEquatable<ContentTab?>
{
    public string Name { get; }

    public Content Content { get; }

    public IContentPanel Parent { get; }

    public ContentPanel ParentPanel { get; }

    public ContentTab(string name, Content content, ContentPanel parent)
    {
        Name = name;
        Content = content;
        Parent = parent;
        ParentPanel = parent;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ContentTab);
    }

    public bool Equals(ContentTab? other)
    {
        return other is not null &&
               Name == other.Name &&
               EqualityComparer<Content>.Default.Equals(Content, other.Content) &&
               EqualityComparer<ContentPanel>.Default.Equals(ParentPanel, other.ParentPanel);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Content, ParentPanel);
    }
}
