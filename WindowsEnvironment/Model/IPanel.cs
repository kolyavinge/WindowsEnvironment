using System.ComponentModel;

namespace WindowsEnvironment.Model;

public interface IPanel : INotifyPropertyChanged
{
    string Name { get; }

    ILayoutPanel? Parent { get; }
}
