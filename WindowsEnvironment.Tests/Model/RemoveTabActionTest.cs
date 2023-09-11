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
        _rootPanel.Children.Add(_mainPanel);
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
        var tab = _mainPanel.Tab.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.Children, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.Children[0], Is.EqualTo(_mainPanel));
        Assert.That(_mainPanel.Tab, Has.Count.EqualTo(0));
        _events.Verify(x => x.RaiseTabRemoved(null, _mainPanel, tab, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromRootPanel()
    {
        var tab1 = _mainPanel.Tab.Add(_content);
        var tab2 = _mainPanel.Tab.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab2.Name)).Returns(tab2);

        _action.RemoveTab(tab2.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.Children, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.Children[0], Is.EqualTo(_mainPanel));
        Assert.That(_mainPanel.Tab, Has.Count.EqualTo(1));
        Assert.That(_mainPanel.Tab[0], Is.EqualTo(tab1));
        _events.Verify(x => x.RaiseTabRemoved(null, _mainPanel, tab2, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveLastTabFromPanel()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        var tab = panel1.Tab.Add(_content);
        _rootPanel.Children.Add(panel1);
        panel1.Parent = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);
        _parentsChainFinder.Setup(x => x.FindChain(panel1.Name)).Returns(new List<LayoutPanel> { _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.Children, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.Children[0], Is.EqualTo(_mainPanel));
        _events.Verify(x => x.RaiseTabRemoved(panel1, panel1, tab, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromPanel()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        var tab1 = panel1.Tab.Add(_content);
        var tab2 = panel1.Tab.Add(_content);
        _rootPanel.Children.Add(panel1);
        panel1.Parent = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab2.Name)).Returns(tab2);
        _parentsChainFinder.Setup(x => x.FindChain(panel1.Name)).Returns(new List<LayoutPanel> { _rootPanel });

        _action.RemoveTab(tab2.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.Children, Has.Count.EqualTo(2));
        Assert.That(_rootPanel.Children[0], Is.EqualTo(_mainPanel));
        Assert.That(_rootPanel.Children[1], Is.EqualTo(panel1));
        Assert.That(panel1.Tab, Has.Count.EqualTo(1));
        Assert.That(panel1.Tab[0], Is.EqualTo(tab1));
        _events.Verify(x => x.RaiseTabRemoved(null, panel1, tab2, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromPanelWithEmptyParent()
    {
        var panel1 = new LayoutPanel("panel_1");
        var panel2 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        _rootPanel.Children.Add(panel1);
        panel1.Children.Add(panel2);
        var tab = panel2.Tab.Add(_content);
        panel1.Parent = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);
        _parentsChainFinder.Setup(x => x.FindChain(panel2.Name)).Returns(new List<LayoutPanel> { panel1, _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.Children, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.Children[0], Is.EqualTo(_mainPanel));
        _events.Verify(x => x.RaiseTabRemoved(panel1, panel2, tab, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromPanelWithEmptyParents()
    {
        var panel1 = new LayoutPanel("panel_1");
        var panel2 = new LayoutPanel("panel_2");
        var panel3 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        _rootPanel.Children.Add(panel1);
        panel1.Children.Add(panel2);
        panel2.Children.Add(panel3);
        var tab = panel3.Tab.Add(_content);
        panel1.Parent = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);
        _parentsChainFinder.Setup(x => x.FindChain(panel3.Name)).Returns(new List<LayoutPanel> { panel2, panel1, _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.Children, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.Children[0], Is.EqualTo(_mainPanel));
        _events.Verify(x => x.RaiseTabRemoved(panel1, panel3, tab, RemoveTabMode.Close));
    }

    [Test]
    public void TabClose_CloseCallbackInvoke()
    {
        var invoke = false;
        _content.CloseCallback = () => invoke = true;
        var tab = _mainPanel.Tab.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(invoke, Is.True);
    }

    [Test]
    public void TabUnset_CloseCallbackNotInvoke()
    {
        var invoke = false;
        _content.CloseCallback = () => invoke = true;
        var tab = _mainPanel.Tab.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);

        _action.RemoveTab(tab.Name, RemoveTabMode.Unset);

        Assert.That(invoke, Is.False);
    }

    [Test]
    public void TabUnset_AddFlexPanel()
    {
        var tab = _mainPanel.Tab.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object));
        _panelFactory.Setup(p => p.MakeNewContentPanel()).Returns(flexPanel);

        _action.RemoveTab(tab.Name, RemoveTabMode.Unset);

        _panelFactory.Verify(x => x.MakeNewContentPanel(), Times.Once);
        _panels.Verify(x => x.AddFlexPanel(flexPanel), Times.Once);
        Assert.That(flexPanel.State, Is.EqualTo(PanelState.Flex));
        Assert.That(flexPanel.Tab, Has.Count.EqualTo(1));
    }

    [Test]
    public void RemoveFlexPanel()
    {
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object)) { State = PanelState.Flex };
        var tab = flexPanel.Tab.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        _panels.Verify(x => x.RemoveFlexPanelTabById(_contentId), Times.Once());
        _events.Verify(x => x.RaiseTabRemoved(null, flexPanel, tab, RemoveTabMode.Close), Times.Once());
    }

    [Test]
    public void RemoveFlexPanel_CloseCallbackInvoke()
    {
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object)) { State = PanelState.Flex };
        var tab = flexPanel.Tab.Add(_content);
        var invoke = false;
        _content.CloseCallback = () => invoke = true;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(invoke, Is.True);
    }

    [Test]
    public void RemoveFlexPanel_CloseCallbackNull_NotInvoke()
    {
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object)) { State = PanelState.Flex };
        var tab = flexPanel.Tab.Add(_content);
        _content.CloseCallback = null;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);
    }

    [Test]
    public void RemoveFlexPanel_Uset_Error()
    {
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object)) { State = PanelState.Flex };
        var tab = flexPanel.Tab.Add(_content);
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);
        try
        {
            _action.RemoveTab(tab.Name, RemoveTabMode.Unset);
            Assert.Fail();
        }
        catch (ArgumentException)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void RemoveTabResetSizeBeforePanel()
    {
        var panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object));
        panel1.Size = 100;
        var panel2 = new ContentPanel("panel_2", new(_nameGenerator.Object));
        var tab = panel2.Tab.Add(_content);
        _rootPanel.Children.Add(panel1);
        _rootPanel.Children.Add(panel2);
        panel2.Parent = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);
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
        var tab = panel1.Tab.Add(_content);
        _rootPanel.Children.Add(panel1);
        _rootPanel.Children.Add(panel2);
        panel1.Parent = _rootPanel;
        _panels.Setup(x => x.GetTabByName(tab.Name)).Returns(tab);
        _parentsChainFinder.Setup(x => x.FindChain(panel1.Name)).Returns(new List<LayoutPanel> { _rootPanel });

        _action.RemoveTab(tab.Name, RemoveTabMode.Close);

        Assert.That(panel2.Size, Is.Null);
    }
}
