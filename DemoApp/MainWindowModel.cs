using WindowsEnvironment;
using WindowsEnvironment.Model;

namespace DemoApp;

public class MainWindowModel
{
    public IFlexWindowsEnvironment Model { get; }

    public MainWindowModel()
    {
        Model = FlexWindowsEnvironmentFactory.Make();
    }
}
