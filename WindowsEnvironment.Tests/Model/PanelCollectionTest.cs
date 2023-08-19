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
        _rootPanel = new Panel(MainPanel.Name, new(_nameGenerator.Object));
        _panelFactory = new Mock<IPanelFactory>();
        _panelFactory.Setup(x => x.MakeNew()).Returns(_rootPanel);
        _panels = new PanelCollection(_panelFactory.Object);
    }

    [Test]
    public void Constructor()
    {
        Assert.That(_panels.RootPanel, Is.Not.Null);
    }

    [Test]
    public void GetPanelByName()
    {
        Assert.That(_panels.GetPanelByName(MainPanel.Name), Is.EqualTo(_rootPanel));
        _panelFactory.Verify(x => x.MakeNew(), Times.Once);
    }

    [Test]
    public void GetPanelByName_WrongName_Error()
    {
        try
        {
            _panels.GetPanelByName("wrong_name");
            Assert.Fail();
        }
        catch (ArgumentException ex)
        {
            Assert.That(ex.Message, Is.EqualTo("'wrong_name' does not exist."));
        }
    }

    [Test]
    public void GetTabByName()
    {
        _nameGenerator.Setup(x => x.GetContentTabName()).Returns("tab_1");
        var tab = _rootPanel.ContentTabCollection.Add(new("id"));

        var (resultPanel, resultTab) = _panels.GetTabByName("tab_1");

        Assert.That(resultPanel, Is.EqualTo(_rootPanel));
        Assert.That(tab, Is.EqualTo(resultTab));
    }

    [Test]
    public void GetTabByName_WrongName_Error()
    {
        try
        {
            _panels.GetTabByName("wrong_name");
            Assert.Fail();
        }
        catch (ArgumentException ex)
        {
            Assert.That(ex.Message, Is.EqualTo("'wrong_name' does not exist."));
        }
    }

    [Test]
    public void GetTabById()
    {
        _nameGenerator.Setup(x => x.GetContentTabName()).Returns("tab_1");
        var tab = _rootPanel.ContentTabCollection.Add(new("id"));

        var (resultPanel, resultTab) = _panels.GetTabById("id");

        Assert.That(resultPanel, Is.EqualTo(_rootPanel));
        Assert.That(tab, Is.EqualTo(resultTab));
    }

    [Test]
    public void GetTabById_WrongId_Error()
    {
        try
        {
            _panels.GetTabById("wrong_id");
            Assert.Fail();
        }
        catch (ArgumentException ex)
        {
            Assert.That(ex.Message, Is.EqualTo("Tab with id 'wrong_id' does not exist."));
        }
    }
}
