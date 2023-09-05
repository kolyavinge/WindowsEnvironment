using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class SelectTabActionTest
{
    private object _contentId;
    private Content _content;
    private Mock<INameGenerator> _nameGenerator;
    private ContentPanel _panel;
    private Mock<IPanelCollection> _panels;
    private Mock<IEventsInternal> _events;
    private SelectTabAction _action;

    [SetUp]
    public void Setup()
    {
        _contentId = new object();
        _content = new Content(_contentId);
        _nameGenerator = new Mock<INameGenerator>();
        _nameGenerator.SetupSequence(x => x.GetContentTabName()).Returns("tab_1").Returns("tab_2").Returns("tab_3");
        _panel = new ContentPanel(MainPanel.Name, new(_nameGenerator.Object));
        _panels = new Mock<IPanelCollection>();
        _events = new Mock<IEventsInternal>();
        _action = new SelectTabAction(_panels.Object, _events.Object);
    }

    [Test]
    public void SelectTab()
    {
        _panel.TabCollection.Add(_content);
        var tab = _panel.TabCollection.Add(_content);
        _panel.TabCollection.Add(_content);
        _panels.Setup(x => x.GetTabByName("tab_2")).Returns((_panel, tab));

        Assert.That(_panel.SelectedTabName, Is.Null);
        _action.SelectTab("tab_2");
        Assert.That(_panel.SelectedTabName, Is.EqualTo("tab_2"));
    }

    [Test]
    public void SelectTabTwice()
    {
        _panel.TabCollection.Add(_content);
        var tab = _panel.TabCollection.Add(_content);
        _panel.TabCollection.Add(_content);
        _panels.Setup(x => x.GetTabByName("tab_2")).Returns((_panel, tab));

        _action.SelectTab("tab_2");
        _action.SelectTab("tab_2");
        Assert.That(_panel.SelectedTabName, Is.EqualTo("tab_2"));
    }

    [Test]
    public void SelectTab_RaiseEvent()
    {
        var tab = _panel.TabCollection.Add(_content);
        _panels.Setup(x => x.GetTabByName("tab_1")).Returns((_panel, tab));

        _action.SelectTab("tab_1");
        _events.Verify(x => x.RaiseTabSelected(_panel, tab));
    }

    [Test]
    public void WrongTab_Error()
    {
        try
        {
            _panels.Setup(x => x.GetTabByName("tab_2")).Throws(new ArgumentException("'tab_2' does not exist."));
            _action.SelectTab("tab_2");
            Assert.Fail();
        }
        catch (ArgumentException e)
        {
            Assert.That(e.Message, Is.EqualTo("'tab_2' does not exist."));
        }
    }
}
