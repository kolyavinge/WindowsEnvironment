using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class FlexWindowsEnvironmentReaderTest
{
    private Mock<IFlexWindowsEnvironment> _model;
    private FlexWindowsEnvironmentReader _reader;

    [SetUp]
    public void Setup()
    {
        _model = new Mock<IFlexWindowsEnvironment>();
        _reader = new FlexWindowsEnvironmentReader(_model.Object);
    }

    [Test]
    public void Read()
    {
        var tab1 = new Mock<IContentTab>();
        var panel1 = new Mock<IPanel>();
        var root = new Mock<IPanel>();
        panel1.SetupGet(x => x.Name).Returns("panel_1");
        panel1.SetupGet(x => x.Tabs).Returns(new[] { tab1.Object });
        panel1.SetupGet(x => x.Children).Returns(new IPanel[0]);
        root.SetupGet(x => x.Name).Returns("panel_0");
        root.SetupGet(x => x.Tabs).Returns(new IContentTab[0]);
        root.SetupGet(x => x.Children).Returns(new[] { panel1.Object });
        _model.SetupGet(x => x.RootPanel).Returns(root.Object);
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

        Assert.That(beginReadPanels[0], Is.EqualTo(root.Object));
        Assert.That(beginReadPanels[1], Is.EqualTo(panel1.Object));
        Assert.That(readTabs[0], Is.EqualTo(tab1.Object));
        Assert.That(endReadPanels[0], Is.EqualTo(panel1.Object));
        Assert.That(endReadPanels[1], Is.EqualTo(root.Object));
    }

    [Test]
    public void ReadNoEvents()
    {
        var tab1 = new Mock<IContentTab>();
        var panel1 = new Mock<IPanel>();
        var root = new Mock<IPanel>();
        panel1.SetupGet(x => x.Name).Returns("panel_1");
        panel1.SetupGet(x => x.Tabs).Returns(new[] { tab1.Object });
        panel1.SetupGet(x => x.Children).Returns(new IPanel[0]);
        root.SetupGet(x => x.Name).Returns("panel_0");
        root.SetupGet(x => x.Tabs).Returns(new IContentTab[0]);
        root.SetupGet(x => x.Children).Returns(new[] { panel1.Object });
        _model.SetupGet(x => x.RootPanel).Returns(root.Object);

        _reader.Read();
    }
}
