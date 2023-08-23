using System.Collections;
using System.Collections.Generic;

namespace WindowsEnvironment.Model;

internal class PanelChildrenCollection : IReadOnlyList<Panel>
{
    private readonly List<Panel> _children;

    public int Count => _children.Count;

    public Panel this[int i] => _children[i];

    public PanelChildrenCollection()
    {
        _children = new List<Panel>();
    }

    public void AddBegin(Panel child)
    {
        _children.Insert(0, child);
    }

    public void AddEnd(Panel child)
    {
        _children.Add(child);
    }

    public void Remove(Panel child)
    {
        _children.Remove(child);
    }

    public void Clear()
    {
        _children.Clear();
    }

    public int IndexOf(Panel child)
    {
        return _children.IndexOf(child);
    }

    public PanelChildrenCollection GetCopy()
    {
        var copy = new PanelChildrenCollection();
        foreach (var ch in _children)
        {
            copy.AddEnd(ch);
        }

        return copy;
    }

    public IEnumerator<Panel> GetEnumerator() => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
