using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class ParentsChainFinderTest
{
    private LayoutPanel _rootPanel;
    private Mock<IPanelCollection> _panels;
    private Mock<INameGenerator> _nameGenerator;
    private ParentsChainFinder _finder;

    [SetUp]
    public void Setup()
    {
        _nameGenerator = new Mock<INameGenerator>();
        _rootPanel = new LayoutPanel(MainPanel.Name);
        _panels = new Mock<IPanelCollection>();
        _panels.SetupGet(x => x.RootPanel).Returns(_rootPanel);
        _finder = new ParentsChainFinder(_panels.Object);
    }

    [Test]
    public void FindChainRootOnly()
    {
        var result = _finder.FindChain(MainPanel.Name);

        Assert.That(result, Has.Count.EqualTo(0));
    }

    [Test]
    public void FindChainOneChild()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        _rootPanel.Children.Add(panel1);

        var result = _finder.FindChain(panel1.Name);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.EqualTo(_rootPanel));
    }

    [Test]
    public void FindChainTwoChildren()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        var panel2 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        _rootPanel.Children.Add(panel1);
        _rootPanel.Children.Add(panel2);

        var result = _finder.FindChain(panel1.Name);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.EqualTo(_rootPanel));
    }

    [Test]
    public void FindChainNoResult()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        var panel2 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        _rootPanel.Children.Add(panel1);
        _rootPanel.Children.Add(panel2);

        var result = _finder.FindChain("wrong panel");

        Assert.That(result, Has.Count.EqualTo(0));
    }
}
