using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class SetPanelPositionActionTest
{
    private object _contentId;
    private Content _content;
    private ContentPanel _mainPanel, _panel1, _panel2, _panel3, _panel4;
    private LayoutPanel _panel10, _panel20, _panel30, _panel40;
    private Mock<IPanelCollection> _panels;
    private Mock<IPanelFactory> _panelFactory;
    private Mock<IEventsInternal> _events;
    private Mock<INameGenerator> _nameGenerator;
    private SetPanelPositionAction _action;

    [SetUp]
    public void Setup()
    {
        _contentId = new object();
        _content = new Content(_contentId);
        _panels = new Mock<IPanelCollection>();
        _panelFactory = new Mock<IPanelFactory>();
        _events = new Mock<IEventsInternal>();
        _nameGenerator = new Mock<INameGenerator>();
        _nameGenerator.SetupSequence(x => x.GetContentTabName()).Returns("tab_0").Returns("tab_1").Returns("tab_2").Returns("tab_3");
        _action = new SetPanelPositionAction(_panels.Object, _panelFactory.Object, _events.Object);
        _mainPanel = new ContentPanel(MainPanel.Name, new(_nameGenerator.Object));
        _panelFactory.SetupSequence(x => x.MakeNewContentPanel())
            .Returns(_panel1 = new ContentPanel("panel_1", new(_nameGenerator.Object)))
            .Returns(_panel2 = new ContentPanel("panel_2", new(_nameGenerator.Object)))
            .Returns(_panel3 = new ContentPanel("panel_3", new(_nameGenerator.Object)))
            .Returns(_panel4 = new ContentPanel("panel_4", new(_nameGenerator.Object)));
        _panelFactory.SetupSequence(x => x.MakeNewLayoutPanel())
            .Returns(_panel10 = new LayoutPanel("panel_10"))
            .Returns(_panel20 = new LayoutPanel("panel_20"))
            .Returns(_panel30 = new LayoutPanel("panel_30"))
            .Returns(_panel40 = new LayoutPanel("panel_40"));
        _panels.Setup(x => x.GetPanelByName(_mainPanel.Name)).Returns(_mainPanel);
        _panels.Setup(x => x.GetPanelByName(_panel1.Name)).Returns(_panel1);
        _panels.Setup(x => x.GetPanelByName(_panel2.Name)).Returns(_panel2);
        _panels.Setup(x => x.GetPanelByName(_panel3.Name)).Returns(_panel3);
        _panels.Setup(x => x.GetPanelByName(_panel4.Name)).Returns(_panel4);
        _panels.Setup(x => x.GetPanelByName(_panel10.Name)).Returns(_panel10);
        _panels.Setup(x => x.GetPanelByName(_panel20.Name)).Returns(_panel20);
        _panels.Setup(x => x.GetPanelByName(_panel30.Name)).Returns(_panel30);
        _panels.Setup(x => x.GetPanelByName(_panel40.Name)).Returns(_panel40);
    }

    [Test]
    public void SetPanelPositionLeft_ReturnValue()
    {
        var (panel, tab) = _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Left, _content);

        Assert.That(panel, Is.EqualTo(_panel1));
        Assert.That(tab.Name, Is.EqualTo("tab_0"));
    }

    [Test]
    public void SetPanelPositionMiddle_ReturnValue()
    {
        var (panel, tab) = _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Middle, _content);

        Assert.That(panel, Is.EqualTo(_mainPanel));
        Assert.That(tab.Name, Is.EqualTo("tab_0"));
    }

    [Test]
    public void SetPanelPositionLeft()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Left, _content);

        Assert.That(_panel10.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_panel1));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_mainPanel));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));
    }

    [Test]
    public void SetPanelPositionRight()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);

        Assert.That(_panel10.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_panel1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetPanelPositionTop()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Top, _content);

        Assert.That(_panel10.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_panel1));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_mainPanel));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));
    }

    [Test]
    public void SetPanelPositionBottom()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Bottom, _content);

        Assert.That(_panel10.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_panel1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetPanelPositionLeftLeft()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Left, _content);
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Left, _content);

        Assert.That(_panel10.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(3));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_panel1));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_panel2));
        Assert.That(_panel10.ChildrenList[2], Is.EqualTo(_mainPanel));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));
    }

    [Test]
    public void SetPanelPositionLeftLeft2()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Left, _content);
        _action.SetPanelPosition(_panel10.Name, PanelPosition.Left, _content);

        Assert.That(_panel20.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel20.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel20.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel20.ChildrenList[0], Is.EqualTo(_panel2));
        Assert.That(_panel20.ChildrenList[1], Is.EqualTo(_panel10));

        Assert.That(_panel10.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_panel1));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_mainPanel));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetPanelPositionRightRight()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);
        _action.SetPanelPosition(_panel10.Name, PanelPosition.Right, _content);

        Assert.That(_panel20.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel20.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel20.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel20.ChildrenList[0], Is.EqualTo(_panel10));
        Assert.That(_panel20.ChildrenList[1], Is.EqualTo(_panel2));

        Assert.That(_panel10.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_panel1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetPanelPositionTopTop()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Top, _content);
        _action.SetPanelPosition(_panel10.Name, PanelPosition.Top, _content);

        Assert.That(_panel20.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel20.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel20.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel20.ChildrenList[0], Is.EqualTo(_panel2));
        Assert.That(_panel20.ChildrenList[1], Is.EqualTo(_panel10));

        Assert.That(_panel10.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_panel1));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_mainPanel));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetPanelPositionBottomBottom()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Bottom, _content);
        _action.SetPanelPosition(_panel10.Name, PanelPosition.Bottom, _content);

        Assert.That(_panel20.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel20.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel20.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel20.ChildrenList[0], Is.EqualTo(_panel10));
        Assert.That(_panel20.ChildrenList[1], Is.EqualTo(_panel2));

        Assert.That(_panel10.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_panel1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetPanelPositionLeftTop()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Left, _content);
        _action.SetPanelPosition(_panel10.Name, PanelPosition.Top, _content);

        Assert.That(_panel20.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel20.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel20.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel20.ChildrenList[0], Is.EqualTo(_panel2));
        Assert.That(_panel20.ChildrenList[1], Is.EqualTo(_panel10));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_panel10.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_panel1));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_mainPanel));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));
    }

    [Test]
    public void SetPanelPositionRightBottom()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);
        _action.SetPanelPosition(_panel10.Name, PanelPosition.Bottom, _content);

        Assert.That(_panel20.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel20.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel20.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel20.ChildrenList[0], Is.EqualTo(_panel10));
        Assert.That(_panel20.ChildrenList[1], Is.EqualTo(_panel2));

        Assert.That(_panel10.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_panel1));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetPanelPositionRightRightInChildPanel()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);
        _action.SetPanelPosition(_panel1.Name, PanelPosition.Right, _content);

        Assert.That(_panel10.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(3));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_panel1));
        Assert.That(_panel10.ChildrenList[2], Is.EqualTo(_panel2));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetPanelPositionRightBottomInChildPanel()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);
        _action.SetPanelPosition(_panel1.Name, PanelPosition.Bottom, _content);

        Assert.That(_panel10.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_mainPanel));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_panel20));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));

        Assert.That(_panel20.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel20.Orientation, Is.EqualTo(PanelOrientation.ByRows));
        Assert.That(_panel20.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel20.ChildrenList[0], Is.EqualTo(_panel1));
        Assert.That(_panel20.ChildrenList[1], Is.EqualTo(_panel2));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(1));

        Assert.That(_panel2.ParentPanel, Is.EqualTo(_panel20));
        Assert.That(_panel2.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetRootPanelMiddle()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Middle, _content);

        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(1));
    }

    [Test]
    public void SetLeftPanelMiddle()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Left, _content);
        _action.SetPanelPosition(_panel1.Name, PanelPosition.Middle, _content);

        Assert.That(_panel10.ParentPanel, Is.EqualTo(null));
        Assert.That(_panel10.Orientation, Is.EqualTo(PanelOrientation.ByCols));
        Assert.That(_panel10.ChildrenList, Has.Count.EqualTo(2));
        Assert.That(_panel10.ChildrenList[0], Is.EqualTo(_panel1));
        Assert.That(_panel10.ChildrenList[1], Is.EqualTo(_mainPanel));

        Assert.That(_panel1.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_panel1.TabCollection, Has.Count.EqualTo(2));

        Assert.That(_mainPanel.ParentPanel, Is.EqualTo(_panel10));
        Assert.That(_mainPanel.TabCollection, Has.Count.EqualTo(0));
    }

    [Test]
    public void SetMiddleWrongPanel()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);
        try
        {
            _action.SetPanelPosition(_panel10.Name, PanelPosition.Middle, _content);
            Assert.Fail();
        }
        catch (ArgumentException e)
        {
            Assert.That(e.Message, Is.EqualTo("panel_10 does not contain tabs."));
        }
    }

    [Test]
    public void PanelAddedRaise()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Left, _content);

        _events.Verify(x => x.RaisePanelAdded(_panel10, _panel1, new("tab_0", _content)));
    }

    [Test]
    public void ChildPanelAddedRaise()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);

        _events.Verify(x => x.RaiseParentChanged(_panel10, _mainPanel));
    }

    [Test]
    public void TabAddedRaise()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Middle, _content);

        _events.Verify(x => x.RaiseTabAdded(_mainPanel, new("tab_0", _content)));
    }

    [Test]
    public void SetPanelPositionSetRoot()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);

        _panels.Verify(x => x.SetRoot(_panel10), Times.Once());
    }

    [Test]
    public void SetPanelPosition_RemoveFlexPanelTabById()
    {
        _action.SetPanelPosition(_mainPanel.Name, PanelPosition.Right, _content);

        _panels.Verify(x => x.RemoveFlexPanelTabById(_contentId), Times.Once);
    }
}
