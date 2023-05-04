namespace WindowsEnvironment.Model;

internal interface IPanelFactory
{
    Panel MakeNew();
}

internal class PanelFactory : IPanelFactory
{
    private readonly INameGenerator _nameGenerator;

    public PanelFactory(INameGenerator nameGenerator)
    {
        _nameGenerator = nameGenerator;
    }

    public Panel MakeNew()
    {
        return new Panel(_nameGenerator.GetPanelName(), new(_nameGenerator));
    }
}
