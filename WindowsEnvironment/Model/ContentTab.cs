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
    #region IContentTab
    IContentPanel IContentTab.Parent => Parent;
    #endregion

    public string Name { get; }

    public Content Content { get; }

    public ContentPanel Parent { get; }

    public ContentTab(string name, Content content, ContentPanel parent)
    {
        Name = name;
        Content = content;
        Parent = parent;
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
               EqualityComparer<ContentPanel>.Default.Equals(Parent, other.Parent);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Content, Parent);
    }
}
