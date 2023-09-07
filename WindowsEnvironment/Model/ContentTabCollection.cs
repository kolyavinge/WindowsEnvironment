using System.Collections;
using System.Collections.Generic;

namespace WindowsEnvironment.Model;

internal class ContentTabCollection : IReadOnlyCollection<ContentTab>
{
    private readonly List<ContentTab> _tabs;
    private readonly INameGenerator _nameGenerator;

    public ContentPanel? ParentPanel;

    public int Count => _tabs.Count;

    public ContentTab this[int i] => _tabs[i];

    public ContentTabCollection(INameGenerator nameGenerator)
    {
        _tabs = new List<ContentTab>();
        _nameGenerator = nameGenerator;
    }

    public ContentTab Add(Content content)
    {
        if (ParentPanel == null) throw new InvalidOperationException("ParentPanel has not been initialized.");

        var tab = new ContentTab(_nameGenerator.GetContentTabName(), content, ParentPanel!);
        _tabs.Add(tab);

        return tab;
    }

    public void Remove(ContentTab tab)
    {
        _tabs.Remove(tab);
    }

    public IEnumerator<ContentTab> GetEnumerator() => _tabs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _tabs.GetEnumerator();
}
