using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class RemoveTabActionTest
{
    private object _contentId;
    private Content _content;
    private Mock<INameGenerator> _nameGenerator;
    private LayoutPanel _rootPanel;
    private ContentPanel _mainPanel;
    private Mock<IPanelFactory> _panelFactory;
    private Mock<IPanelCollection> _panels;
    private Mock<IParentsChainFinder> _parentsChainFinder;
    private Mock<IEventsInternal> _events;
    private RemoveTabAction _action;

    [SetUp]
    public void Setup()
    {
        _contentId = new object();
        _content = new Content(_contentId);
        _nameGenerator = new Mock<INameGenerator>();
        _mainPanel = new ContentPanel(MainPanel.Name, new(_nameGenerator.Object));
        _rootPanel = new LayoutPanel("panel_1");
        _rootPanel.ChildrenList.Add(_mainPanel);
        _panelFactory = new Mock<IPanelFactory>();
        _panelFactory.Setup(p => p.MakeNewContentPanel()).Returns(new ContentPanel("", new(_nameGenerator.Object)));
        _panels = new Mock<IPanelCollection>();
        _panels.SetupGet(x => x.RootPanel).Returns(_rootPanel);
        _parentsChainFinder = new Mock<IParentsChainFinder>();
        _events = new Mock<IEventsInternal>();
        _action = new RemoveTabAction(_panelFactory.Object, _panels.Object, _parentsChainFinder.Object, _events.Object);
    }

    [Test]
    public void RemoveWrongTabFromRootPanel()
    {
        _panels.Setup(x => x.GetTabByName("wrong name")).Throws(new ArgumentException("'wrong name' does not exist."));
        try
        {
            _action.RemoveTab("wrong name", RemoveTabMode.Close);
            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.That(e.Message, Is.EqualTo("'wrong name' does not exist."));
        }
    }

    [Test]
    public void RemoveLastTabFromRootPanel()
    {
        var tab = _mainPanel.TabCollection.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((_mainPanel, tab));

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ChildrenList, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));
        _events.Verify(x => x.RaiseTabRemoved(null, _mainPanel, tab, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromRootPanel()
    {
        var tab1 = _mainPanel.TabCollection.Add(_content);
        var tab2 = _mainPanel.TabCollection.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab2.Name)).Returns((_mainPanel, tab2));

        _action.RemoveTab(tab2.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ChildrenList, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(1));
        Assert.That(_mainPanel.TabCollection[0], Is.EqualTo(tab1));
        _events.Verify(x => x.RaiseTabRemoved(null, _mainPanel, tab2, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveLastTabFromPanel()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        var tab = panel1.TabCollection.Add(_content);
        _rootPanel.ChildrenList.Add(panel1);
        panel1.ParentPanel = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((panel1, tab));
        _parentsChainFinder.Setup(x => x.FindChain(panel1.Name)).Returns(new List<LayoutPanel> { _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ChildrenList, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.ChildrenList[0], Is.EqualTo(_mainPanel));
        _events.Verify(x => x.RaiseTabRemoved(panel1, panel1, tab, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromPanel()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        var tab1 = panel1.TabCollection.Add(_content);
        var tab2 = panel1.TabCollection.Add(_content);
        _rootPanel.ChildrenList.Add(panel1);
        panel1.ParentPanel = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab2.Name)).Returns((panel1, tab2));
        _parentsChainFinder.Setup(x => x.FindChain(panel1.Name)).Returns(new List<LayoutPanel> { _rootPanel });

        _action.RemoveTab(tab2.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_rootPanel.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_rootPanel.ChildrenList[1], Is.EqualTo(panel1));
        Assert.That(panel1.TabCollection, Has.Count.EqualTo(1));
        Assert.That(panel1.TabCollection[0], Is.EqualTo(tab1));
        _events.Verify(x => x.RaiseTabRemoved(null, panel1, tab2, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromPanelWithEmptyParent()
    {
        var panel1 = new LayoutPanel("panel_1");
        var panel2 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        _rootPanel.ChildrenList.Add(panel1);
        panel1.ChildrenList.Add(panel2);
        var tab = panel2.TabCollection.Add(_content);
        panel1.ParentPanel = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((panel2, tab));
        _parentsChainFinder.Setup(x => x.FindChain(panel2.Name)).Returns(new List<LayoutPanel> { panel1, _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ChildrenList, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.ChildrenList[0], Is.EqualTo(_mainPanel));
        _events.Verify(x => x.RaiseTabRemoved(panel1, panel2, tab, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromPanelWithEmptyParents()
    {
        var panel1 = new LayoutPanel("panel_1");
        var panel2 = new LayoutPanel("panel_2");
        var panel3 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        _rootPanel.ChildrenList.Add(panel1);
        panel1.ChildrenList.Add(panel2);
        panel2.ChildrenList.Add(panel3);
        var tab = panel3.TabCollection.Add(_content);
        panel1.ParentPanel = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((panel3, tab));
        _parentsChainFinder.Setup(x => x.FindChain(panel3.Name)).Returns(new List<LayoutPanel> { panel2, panel1, _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ChildrenList, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.ChildrenList[0], Is.EqualTo(_mainPanel));
        _events.Verify(x => x.RaiseTabRemoved(panel1, panel3, tab, RemoveTabMode.Close));
    }

    [Test]
    public void TabClose_CloseCallbackInvoke()
    {
        var invoke = false;
        _content.CloseCallback = () => invoke = true;
        var tab = _mainPanel.TabCollection.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((_mainPanel, tab));

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(invoke, Is.True);
    }

    [Test]
    public void TabUnset_CloseCallbackNotInvoke()
    {
        var invoke = false;
        _content.CloseCallback = () => invoke = true;
        var tab = _mainPanel.TabCollection.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((_mainPanel, tab));

        _action.RemoveTab(tab.Name, RemoveTabMode.Unset);

        Assert.That(invoke, Is.False);
    }

    [Test]
    public void TabUnset_AddFlexPanel()
    {
        var tab = _mainPanel.TabCollection.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((_mainPanel, tab));
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object));
        _panelFactory.Setup(p => p.MakeNewContentPanel()).Returns(flexPanel);

        _action.RemoveTab(tab.Name, RemoveTabMode.Unset);

        _panelFactory.Verify(x => x.MakeNewContentPanel(), Times.Once);
        _panels.Verify(x => x.AddFlexPanel(flexPanel), Times.Once);
        Assert.That(flexPanel.State, Is.EqualTo(PanelState.Flex));
        Assert.That(flexPanel.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void RemoveTabResetSizeBeforePanel()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        panel1.Size = 100;
        var panel2 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        var tab = panel2.TabCollection.Add(_content);
        _rootPanel.ChildrenList.Add(panel1);
        _rootPanel.ChildrenList.Add(panel2);
        panel2.ParentPanel = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((panel2, tab));
        _parentsChainFinder.Setup(x => x.FindChain(panel2.Name)).Returns(new List<LayoutPanel> { _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(panel1.Size, Is.Null);
    }

    [Test]
    public void RemoveTabResetSizeAfterPanel()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        var panel2 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        panel2.Size = 100;
        var tab = panel1.TabCollection.Add(_content);
        _rootPanel.ChildrenList.Add(panel1);
        _rootPanel.ChildrenList.Add(panel2);
        panel1.ParentPanel = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns((panel1, tab));
        _parentsChainFinder.Setup(x => x.FindChain(panel1.Name)).Returns(new List<LayoutPanel> { _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(panel2.Size, Is.Null);
    }
}
