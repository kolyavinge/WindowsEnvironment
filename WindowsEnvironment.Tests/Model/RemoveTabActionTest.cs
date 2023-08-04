﻿using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class RemoveTabActionTest
{
    private Content _content;
    private Mock<INameGenerator> _nameGenerator;
    private Panel _rootPanel;
    private Mock<IPanelCollection> _panels;
    private Mock<IParentsChainFinder> _parentsChainFinder;
    private Mock<IEventsInternal> _events;
    private RemoveTabAction _action;

    [SetUp]
    public void Setup()
    {
        _content = new Content();
        _nameGenerator = new Mock<INameGenerator>();
        _rootPanel = new Panel(MainPanel.Name, new(_nameGenerator.Object));
        _panels = new Mock<IPanelCollection>();
        _panels.SetupGet(x => x.RootPanel).Returns(_rootPanel);
        _panels.Setup(x => x.GetPanelByName(MainPanel.Name)).Returns(_rootPanel);
        _parentsChainFinder = new Mock<IParentsChainFinder>();
        _events = new Mock<IEventsInternal>();
        _action = new RemoveTabAction(_panels.Object, _parentsChainFinder.Object, _events.Object);
    }

    [Test]
    public void RemoveWrongTabFromRootPanel()
    {
        _rootPanel.ContentTabCollection.Add(_content);
        try
        {
            _action.RemoveTab(_rootPanel.Name, "wrong name", RemoveTabMode.Close);
            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.That(e.Message, Is.EqualTo("'panel_0' does not contain 'wrong name'."));
        }
    }

    [Test]
    public void RemoveLastTabFromRootPanel()
    {
        var tab = _rootPanel.ContentTabCollection.Add(_content);

        _action.RemoveTab(_rootPanel.Name, tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ContentTabCollection, Has.Count.EqualTo(0));
        _events.Verify(x => x.RaiseTabRemoved(null, _rootPanel, tab, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromRootPanel()
    {
        var tab1 = _rootPanel.ContentTabCollection.Add(_content);
        var tab2 = _rootPanel.ContentTabCollection.Add(_content);

        _action.RemoveTab(_rootPanel.Name, tab2.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ContentTabCollection, Has.Count.EqualTo(1));
        Assert.That(_rootPanel.ContentTabCollection.First(), Is.EqualTo(tab1));
        _events.Verify(x => x.RaiseTabRemoved(null, _rootPanel, tab2, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromEmptyPanel()
    {
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        var panel2 = new Panel("panel_2", new(_nameGenerator.Object));
        var tab = panel2.ContentTabCollection.Add(_content);
        _rootPanel.ChildrenCollection.AddBegin(panel1);
        panel1.ChildrenCollection.AddBegin(panel2);
        _panels.Setup(x => x.GetPanelByName(panel2.Name)).Returns(panel2);
        _parentsChainFinder.Setup(x => x.FindChain(panel2.Name)).Returns(new List<Panel> { panel2, panel1, _rootPanel });

        _action.RemoveTab(panel2.Name, tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ContentTabCollection, Has.Count.EqualTo(0));
        _events.Verify(x => x.RaiseTabRemoved(new(_rootPanel, panel1), panel2, tab, RemoveTabMode.Close));
    }

    [Test]
    public void RemoveTabFromEmptyPanel2()
    {
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        var panel2 = new Panel("panel_2", new(_nameGenerator.Object));
        var panel3 = new Panel("panel_3", new(_nameGenerator.Object));
        _rootPanel.ChildrenCollection.AddBegin(panel1);
        _rootPanel.ChildrenCollection.AddBegin(new Panel("panel_4", new(_nameGenerator.Object)));
        panel1.ChildrenCollection.AddBegin(panel2);
        panel1.ChildrenCollection.AddBegin(panel3);
        var tab = panel3.ContentTabCollection.Add(_content);
        _panels.Setup(x => x.GetPanelByName(panel3.Name)).Returns(panel3);
        _parentsChainFinder.Setup(x => x.FindChain(panel3.Name)).Returns(new List<Panel> { panel3, panel1, _rootPanel });

        _action.RemoveTab(panel3.Name, tab.Name, RemoveTabMode.Close);

        Assert.That(_rootPanel.ChildrenCollection, Has.Count.EqualTo(2));
        Assert.That(panel1.ChildrenCollection, Has.Count.EqualTo(1));
        Assert.That(panel1.ChildrenCollection.First(), Is.EqualTo(panel2));
        _events.Verify(x => x.RaiseTabRemoved(new(panel1, panel3), panel3, tab, RemoveTabMode.Close));
    }

    [Test]
    public void TabClose_CloseCallbackInvoke()
    {
        var invoke = false;
        _content.CloseCallback = () => invoke = true;
        var tab = _rootPanel.ContentTabCollection.Add(_content);

        _action.RemoveTab(_rootPanel.Name, tab.Name, RemoveTabMode.Close);

        Assert.That(invoke, Is.True);
    }

    [Test]
    public void TabUnset_CloseCallbackNotInvoke()
    {
        var invoke = false;
        _content.CloseCallback = () => invoke = true;
        var tab = _rootPanel.ContentTabCollection.Add(_content);

        _action.RemoveTab(_rootPanel.Name, tab.Name, RemoveTabMode.Unset);

        Assert.That(invoke, Is.False);
    }

    [Test]
    public void RemoveTabResetSizeBeforePanel()
    {
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        panel1.Size = 100;
        var panel2 = new Panel("panel_2", new(_nameGenerator.Object));
        var tab = panel2.ContentTabCollection.Add(_content);
        _rootPanel.ChildrenCollection.AddEnd(panel1);
        _rootPanel.ChildrenCollection.AddEnd(panel2);
        _panels.Setup(x => x.GetPanelByName(panel2.Name)).Returns(panel2);
        _parentsChainFinder.Setup(x => x.FindChain(panel2.Name)).Returns(new List<Panel> { panel2, _rootPanel });

        _action.RemoveTab(panel2.Name, tab.Name, RemoveTabMode.Close);
        Assert.That(panel1.Size, Is.Null);
    }

    [Test]
    public void RemoveTabResetSizeAfterPanel()
    {
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        var panel2 = new Panel("panel_2", new(_nameGenerator.Object));
        panel2.Size = 100;
        var tab = panel1.ContentTabCollection.Add(_content);
        _rootPanel.ChildrenCollection.AddEnd(panel1);
        _rootPanel.ChildrenCollection.AddEnd(panel2);
        _panels.Setup(x => x.GetPanelByName(panel1.Name)).Returns(panel1);
        _parentsChainFinder.Setup(x => x.FindChain(panel1.Name)).Returns(new List<Panel> { panel1, _rootPanel });

        _action.RemoveTab(panel1.Name, tab.Name, RemoveTabMode.Close);
        Assert.That(panel2.Size, Is.Null);
    }
}
