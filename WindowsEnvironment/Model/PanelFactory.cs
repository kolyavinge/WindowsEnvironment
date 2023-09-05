namespace WindowsEnvironment.Model;

internal interface IPanelFactory
{
    LayoutPanel MakeNewLayoutPanel();

    ContentPanel MakeNewContentPanel();
}

internal class PanelFactory : IPanelFactory
{
    private readonly INameGenerator _nameGenerator;

    public PanelFactory(INameGenerator nameGenerator)
    {
        _nameGenerator = nameGenerator;
    }

    public LayoutPanel MakeNewLayoutPanel()
    {
        return new LayoutPanel(_nameGenerator.GetPanelName());
    }

    public ContentPanel MakeNewContentPanel()
    {
        return new ContentPanel(_nameGenerator.GetPanelName(), new(_nameGenerator));
    }
}
