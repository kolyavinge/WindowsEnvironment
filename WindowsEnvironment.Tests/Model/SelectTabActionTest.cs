﻿using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class SelectTabActionTest
{
    private Content _content;
    private Mock<INameGenerator> _nameGenerator;
    private Panel _rootPanel;
    private Mock<IPanelCollectionInternal> _panels;
    private Mock<IEventsInternal> _events;
    private SelectTabAction _action;

    [SetUp]
    public void Setup()
    {
        _content = new Content();
        _nameGenerator = new Mock<INameGenerator>();
        _nameGenerator.SetupSequence(x => x.GetContentTabName()).Returns("tab_1").Returns("tab_2").Returns("tab_3");
        _rootPanel = new Panel(Panel.MainPanelName, new(_nameGenerator.Object));
        _panels = new Mock<IPanelCollectionInternal>();
        _panels.SetupGet(x => x.RootPanel).Returns(_rootPanel);
        _panels.Setup(x => x.GetPanelByName(Panel.MainPanelName)).Returns(_rootPanel);
        _events = new Mock<IEventsInternal>();
        _action = new SelectTabAction(_panels.Object, _events.Object);
    }

    [Test]
    public void SelectTab()
    {
        _rootPanel.Tabs.Add(_content);
        _rootPanel.Tabs.Add(_content);
        _rootPanel.Tabs.Add(_content);

        Assert.That(_rootPanel.SelectedTabName, Is.Null);
        _action.SelectTab(_rootPanel.Name, "tab_2");
        Assert.That(_rootPanel.SelectedTabName, Is.EqualTo("tab_2"));
    }

    [Test]
    public void SelectTabTwice()
    {
        _rootPanel.Tabs.Add(_content);
        _rootPanel.Tabs.Add(_content);
        _rootPanel.Tabs.Add(_content);

        _action.SelectTab(_rootPanel.Name, "tab_2");
        _action.SelectTab(_rootPanel.Name, "tab_2");
        Assert.That(_rootPanel.SelectedTabName, Is.EqualTo("tab_2"));
    }

    [Test]
    public void SelectTab_RaiseEvent()
    {
        var tab = _rootPanel.Tabs.Add(_content);

        _action.SelectTab(_rootPanel.Name, "tab_1");
        _events.Verify(x => x.RaiseTabSelected(_rootPanel, tab));
    }

    [Test]
    public void WrongTab_Error()
    {
        try
        {
            _action.SelectTab(_rootPanel.Name, "tab_2");
            Assert.Fail();
        }
        catch (ArgumentException e)
        {
            Assert.That(e.Message, Is.EqualTo("'panel_0' does not contain 'tab_2'."));
        }
    }
}