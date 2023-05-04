using WindowsEnvironment.Controllers;
using WindowsEnvironment.Model;

namespace WindowsEnvironment;

public static class MouseControllerFactory
{
    public static IMouseController Make(IFlexWindowsEnvironment model)
    {
        return new MouseController(model);
    }
}
