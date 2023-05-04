using DependencyInjection;
using WindowsEnvironment.Model;

namespace WindowsEnvironment.Infrastructure;

internal class MainInjectModule : InjectModule
{
    public override void Init(IBindingProvider bindingProvider)
    {
        bindingProvider.Bind<IPanelCollectionInternal, PanelCollection>().ToSingleton();
        bindingProvider.Bind<ISetPanelPositionAction, SetPanelPositionAction>().ToSingleton();
        bindingProvider.Bind<IRemoveTabAction, RemoveTabAction>().ToSingleton();
        bindingProvider.Bind<IEventsInternal, Events>().ToSingleton();
        bindingProvider.Bind<IPanelFactory, PanelFactory>().ToSingleton();
        bindingProvider.Bind<INameGenerator, NameGenerator>().ToSingleton();
        bindingProvider.Bind<IParentsChainFinder, ParentsChainFinder>().ToSingleton();
        bindingProvider.Bind<IFlexWindowsEnvironment, FlexWindowsEnvironment>().ToSingleton();
    }
}
