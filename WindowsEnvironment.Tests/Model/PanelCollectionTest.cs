using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class PanelCollectionTest
{
    private Panel _rootPanel;
    private Mock<IPanelFactory> _panelFactory;
    private Mock<INameGenerator> _nameGenerator;
    private PanelCollection _panels;

    [SetUp]
    public void Setup()
    {
        _nameGenerator = new Mock<INameGenerator>();
        _rootPanel = new Panel(Panel.MainPanelName, new(_nameGenerator.Object));
        _panelFactory = new Mock<IPanelFactory>();
        _panelFactory.Setup(x => x.MakeNew()).Returns(_rootPanel);
        _panels = new PanelCollection(_panelFactory.Object);
    }

    [Test]
    public void Constructor()
    {
        Assert.That(_panels.GetPanelByName(Panel.MainPanelName), Is.EqualTo(_rootPanel));
        _panelFactory.Verify(x => x.MakeNew(), Times.Once);
    }
}
