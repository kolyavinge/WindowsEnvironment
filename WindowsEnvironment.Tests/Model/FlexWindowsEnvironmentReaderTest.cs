using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class FlexWindowsEnvironmentReaderTest
{
    private Content _content;
    private Mock<INameGenerator> _nameGenerator;
    private Mock<IFlexWindowsEnvironment> _model;
    private FlexWindowsEnvironmentReader _reader;

    [SetUp]
    public void Setup()
    {
        _content = new Content();
        _nameGenerator = new Mock<INameGenerator>();
        _nameGenerator.Setup(x => x.GetContentTabName()).Returns("tab1");
        _model = new Mock<IFlexWindowsEnvironment>();
        _reader = new FlexWindowsEnvironmentReader(_model.Object);
    }

    [Test]
    public void Read()
    {
        var root = new Panel("panel_0", new(_nameGenerator.Object));
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        var content = new object();
        panel1.ContentTabCollection.Add(_content);
        root.ChildrenCollection.AddBegin(panel1);
        _model.SetupGet(x => x.RootPanel).Returns(root);
        var beginReadPanels = new List<IPanel>();
        var endReadPanels = new List<IPanel>();
        var readTabs = new List<IContentTab>();
        _reader.BeginPanelRead += (s, e) =>
        {
            beginReadPanels.Add(e.Panel);
        };
        _reader.TabRead += (s, e) =>
        {
            readTabs.Add(e.Tab);
        };
        _reader.EndPanelRead += (s, e) =>
        {
            endReadPanels.Add(e.Panel);
        };

        _reader.Read();

        Assert.That(beginReadPanels[0], Is.EqualTo(root));
        Assert.That(beginReadPanels[1], Is.EqualTo(panel1));
        Assert.That(readTabs[0], Is.EqualTo(new ContentTab("tab1", _content)));
        Assert.That(endReadPanels[0], Is.EqualTo(panel1));
        Assert.That(endReadPanels[1], Is.EqualTo(root));
    }

    [Test]
    public void ReadNoEvents()
    {
        var root = new Panel("panel_0", new(_nameGenerator.Object));
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        var content = new object();
        panel1.ContentTabCollection.Add(_content);
        root.ChildrenCollection.AddBegin(panel1);
        _model.SetupGet(x => x.RootPanel).Returns(root);

        _reader.Read();
    }
}
