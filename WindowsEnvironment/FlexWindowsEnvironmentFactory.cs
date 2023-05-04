using DependencyInjection;
using WindowsEnvironment.Infrastructure;
using WindowsEnvironment.Model;

namespace WindowsEnvironment;

public static class FlexWindowsEnvironmentFactory
{
    public static IFlexWindowsEnvironment Make()
    {
        var container = DependencyContainerFactory.MakeLiteContainer();
        container.InitFromModules(new MainInjectModule());

        return container.Resolve<IFlexWindowsEnvironment>();
    }
}
