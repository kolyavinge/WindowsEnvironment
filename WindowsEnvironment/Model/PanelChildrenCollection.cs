using System.Collections;
using System.Collections.Generic;

namespace WindowsEnvironment.Model;

public class PanelChildrenCollection : IReadOnlyList<Panel>
{
    private readonly List<Panel> _children;

    public int Count => _children.Count;

    public Panel this[int i] => _children[i];

    public PanelChildrenCollection()
    {
        _children = new List<Panel>();
    }

    internal void AddBegin(Panel child)
    {
        _children.Insert(0, child);
    }

    internal void AddEnd(Panel child)
    {
        _children.Add(child);
    }

    internal void Remove(Panel child)
    {
        _children.Remove(child);
    }

    internal void Clear()
    {
        _children.Clear();
    }

    internal int IndexOf(Panel child)
    {
        return _children.IndexOf(child);
    }

    internal PanelChildrenCollection GetCopy()
    {
        var copy = new PanelChildrenCollection();
        foreach (var ch in _children)
        {
            copy.AddEnd(ch);
        }

        return copy;
    }

    public IEnumerator<Panel> GetEnumerator()
    {
        return _children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _children.GetEnumerator();
    }
}
