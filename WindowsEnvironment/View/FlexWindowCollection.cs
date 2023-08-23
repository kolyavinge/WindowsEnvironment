using System.Collections;
using System.Collections.Generic;
using System.Windows;
using WindowsEnvironment.Utils;

namespace WindowsEnvironment.View;

internal class FlexWindowCollection : IEnumerable<FlexWindow>
{
    private readonly Application _application;

    public FlexWindowCollection(Application application)
    {
        _application = application;
    }

    public IEnumerator<FlexWindow> GetEnumerator() => _application.Windows.GetFlexWindows().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
