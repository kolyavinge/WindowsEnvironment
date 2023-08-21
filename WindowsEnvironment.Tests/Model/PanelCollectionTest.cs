﻿using WindowsEnvironment.Model;

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
    public void GetEnumerator()
    {
        var panel1 = new Panel("panel1", new(_nameGenerator.Object));
        var panel2 = new Panel("panel2", new(_nameGenerator.Object));
        var panel3 = new Panel("panel3", new(_nameGenerator.Object));
        var flex = new Panel("flex", new(_nameGenerator.Object));
        _rootPanel.ChildrenCollection.AddEnd(panel1);
        panel1.ChildrenCollection.AddEnd(panel2);
        panel2.ChildrenCollection.AddEnd(panel3);
        _panels.AddFlexPanel(flex);

        var result = new List<Panel>(_panels);

        Assert.That(result, Has.Count.EqualTo(5));
        Assert.That(result.Contains(_rootPanel), Is.True);
        Assert.That(result.Contains(panel1), Is.True);
        Assert.That(result.Contains(panel2), Is.True);
        Assert.That(result.Contains(panel3), Is.True);
        Assert.That(result.Contains(flex), Is.True);
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

    [Test]
    public void GetFlexPanelById()
    {
        var flexPanel = new Panel("", new(_nameGenerator.Object));
        flexPanel.ContentTabCollection.Add(new("id"));
        _panels.AddFlexPanel(flexPanel);

        var (resultPanel, resultTab) = _panels.GetFlexPanelById("id");

        Assert.That(resultPanel, Is.EqualTo(flexPanel));
        Assert.That(resultTab, Is.EqualTo(flexPanel.ContentTabCollection.First()));
    }

    [Test]
    public void GetTabById_FlexPanel()
    {
        var flexPanel = new Panel("", new(_nameGenerator.Object));
        flexPanel.ContentTabCollection.Add(new("flex_id"));
        _panels.AddFlexPanel(flexPanel);

        var (resultPanel, resultTab) = _panels.GetTabById("flex_id");

        Assert.That(resultPanel, Is.EqualTo(flexPanel));
        Assert.That(resultTab, Is.EqualTo(flexPanel.ContentTabCollection.First()));
    }

    [Test]
    public void GetFlexPanelById_WrongId_Default()
    {
        var (resultPanel, resultTab) = _panels.GetFlexPanelById("wrong_id");

        Assert.That(resultPanel, Is.Null);
        Assert.That(resultTab, Is.Null);
    }

    [Test]
    public void RemoveFlexPanelTabById_OneTab()
    {
        var flexPanel = new Panel("", new(_nameGenerator.Object));
        flexPanel.ContentTabCollection.Add(new("id"));
        _panels.AddFlexPanel(flexPanel);

        _panels.RemoveFlexPanelTabById("id");

        Assert.That(_panels.FlexPanels, Has.Count.EqualTo(0));
    }

    [Test]
    public void RemoveFlexPanelTabById_TwoTabs()
    {
        var flexPanel = new Panel("", new(_nameGenerator.Object));
        flexPanel.ContentTabCollection.Add(new("id_1"));
        flexPanel.ContentTabCollection.Add(new("id_2"));
        _panels.AddFlexPanel(flexPanel);

        _panels.RemoveFlexPanelTabById("id_1");

        Assert.That(_panels.FlexPanels, Has.Count.EqualTo(1));
        Assert.That(_panels.FlexPanels.First().ContentTabCollection, Has.Count.EqualTo(1));
        Assert.That(_panels.FlexPanels.First().ContentTabCollection.First().Content.Id, Is.EqualTo("id_2"));
    }

    [Test]
    public void RemoveFlexPanelTabById_WrongId()
    {
        var flexPanel = new Panel("", new(_nameGenerator.Object));
        flexPanel.ContentTabCollection.Add(new("id_1"));
        flexPanel.ContentTabCollection.Add(new("id_2"));
        _panels.AddFlexPanel(flexPanel);

        _panels.RemoveFlexPanelTabById("wrong_id");

        Assert.That(_panels.FlexPanels, Has.Count.EqualTo(1));
        Assert.That(_panels.FlexPanels.First().ContentTabCollection, Has.Count.EqualTo(2));
        Assert.That(_panels.FlexPanels.First().ContentTabCollection.First().Content.Id, Is.EqualTo("id_1"));
        Assert.That(_panels.FlexPanels.First().ContentTabCollection.Last().Content.Id, Is.EqualTo("id_2"));
    }
}
