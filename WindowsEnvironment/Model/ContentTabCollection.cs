using System.Collections;
using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public class ContentTabCollection : IReadOnlyCollection<ContentTab>
{
    private readonly List<ContentTab> _tabs;
    private readonly INameGenerator _nameGenerator;

    public int Count => _tabs.Count;

    internal ContentTabCollection(INameGenerator nameGenerator)
    {
        _tabs = new List<ContentTab>();
        _nameGenerator = nameGenerator;
    }

    internal ContentTab AddNew(object content)
    {
        var tab = new ContentTab(_nameGenerator.GetContentTabName(), content);
        _tabs.Add(tab);

        return tab;
    }

    internal void Remove(ContentTab tab)
    {
        _tabs.Remove(tab);
    }

    public IEnumerator<ContentTab> GetEnumerator() => _tabs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _tabs.GetEnumerator();
}
