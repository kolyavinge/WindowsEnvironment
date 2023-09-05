using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class PanelCollectionTest
{
    private LayoutPanel _rootPanel;
    private ContentPanel _mainPanel;
    private Mock<IPanelFactory> _panelFactory;
    private Mock<INameGenerator> _nameGenerator;
    private PanelCollection _panels;

    [SetUp]
    public void Setup()
    {
        _nameGenerator = new Mock<INameGenerator>();
        _rootPanel = new LayoutPanel("panel_1");
        _mainPanel = new ContentPanel(MainPanel.Name, new(_nameGenerator.Object));
        _panelFactory = new Mock<IPanelFactory>();
        _panelFactory.Setup(x => x.MakeNewLayoutPanel()).Returns(_rootPanel);
        _panelFactory.Setup(x => x.MakeNewContentPanel()).Returns(_mainPanel);
        _panels = new PanelCollection(_panelFactory.Object);
    }

    [Test]
    public void Constructor()
    {
        Assert.That(_panels.RootPanel, Is.Not.Null);
        Assert.That(_panels.RootPanel.ChildrenList, Has.Count.EqualTo(1));
        Assert.That(_panels.RootPanel.ChildrenList[0], Is.Not.Null);
    }

    [Test]
    public void GetEnumerator()
    {
        var panel1 = new LayoutPanel("panel1");
        var panel2 = new LayoutPanel("panel2");
        var panel3 = new ContentPanel("panel3", new(_nameGenerator.Object));
        var flex = new ContentPanel("flex", new(_nameGenerator.Object));
        _rootPanel.ChildrenList.Add(panel1);
        panel1.ChildrenList.Add(panel2);
        panel2.ChildrenList.Add(panel3);
        _panels.AddFlexPanel(flex);

        var result = new List<Panel>(_panels);

        Assert.That(result, Has.Count.EqualTo(6));
        Assert.That(result.Contains(_rootPanel), Is.True);
        Assert.That(result.Contains(_mainPanel), Is.True);
        Assert.That(result.Contains(panel1), Is.True);
        Assert.That(result.Contains(panel2), Is.True);
        Assert.That(result.Contains(panel3), Is.True);
        Assert.That(result.Contains(flex), Is.True);
    }

    [Test]
    public void GetPanelByName()
    {
        var panel = _panels.GetPanelByName(MainPanel.Name);
        Assert.That(panel, Is.EqualTo(_mainPanel));
        _panelFactory.Verify(x => x.MakeNewLayoutPanel(), Times.Once);
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
        var panel1 = new ContentPanel("panel1", new(_nameGenerator.Object));
        _rootPanel.ChildrenList.Add(panel1);
        _nameGenerator.Setup(x => x.GetContentTabName()).Returns("tab_1");
        var tab = panel1.TabCollection.Add(new("id"));

        var (resultPanel, resultTab) = _panels.GetTabByName("tab_1");

        Assert.That(resultPanel, Is.EqualTo(panel1));
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
        var panel1 = new ContentPanel("panel1", new(_nameGenerator.Object));
        _rootPanel.ChildrenList.Add(panel1);
        _nameGenerator.Setup(x => x.GetContentTabName()).Returns("tab_1");
        var tab = panel1.TabCollection.Add(new("id"));

        var (resultPanel, resultTab) = _panels.GetTabById("id");

        Assert.That(resultPanel, Is.EqualTo(panel1));
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
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object));
        flexPanel.TabCollection.Add(new("id"));
        _panels.AddFlexPanel(flexPanel);

        var (resultPanel, resultTab) = _panels.GetFlexPanelById("id");

        Assert.That(resultPanel, Is.EqualTo(flexPanel));
        Assert.That(resultTab, Is.EqualTo(flexPanel.TabCollection.First()));
    }

    [Test]
    public void GetTabById_FlexPanel()
    {
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object));
        flexPanel.TabCollection.Add(new("flex_id"));
        _panels.AddFlexPanel(flexPanel);

        var (resultPanel, resultTab) = _panels.GetTabById("flex_id");

        Assert.That(resultPanel, Is.EqualTo(flexPanel));
        Assert.That(resultTab, Is.EqualTo(flexPanel.TabCollection.First()));
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
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object));
        flexPanel.TabCollection.Add(new("id"));
        _panels.AddFlexPanel(flexPanel);

        _panels.RemoveFlexPanelTabById("id");

        Assert.That(_panels.FlexPanels, Has.Count.EqualTo(0));
    }

    [Test]
    public void RemoveFlexPanelTabById_TwoTabs()
    {
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object));
        flexPanel.TabCollection.Add(new("id_1"));
        flexPanel.TabCollection.Add(new("id_2"));
        _panels.AddFlexPanel(flexPanel);

        _panels.RemoveFlexPanelTabById("id_1");

        Assert.That(_panels.FlexPanels, Has.Count.EqualTo(1));
        Assert.That(_panels.FlexPanels.First().TabCollection, Has.Count.EqualTo(1));
        Assert.That(_panels.FlexPanels.First().TabCollection.First().Content.Id, Is.EqualTo("id_2"));
    }

    [Test]
    public void RemoveFlexPanelTabById_WrongId()
    {
        var flexPanel = new ContentPanel("", new(_nameGenerator.Object));
        flexPanel.TabCollection.Add(new("id_1"));
        flexPanel.TabCollection.Add(new("id_2"));
        _panels.AddFlexPanel(flexPanel);

        _panels.RemoveFlexPanelTabById("wrong_id");

        Assert.That(_panels.FlexPanels, Has.Count.EqualTo(1));
        Assert.That(_panels.FlexPanels.First().TabCollection, Has.Count.EqualTo(2));
        Assert.That(_panels.FlexPanels.First().TabCollection.First().Content.Id, Is.EqualTo("id_1"));
        Assert.That(_panels.FlexPanels.First().TabCollection.Last().Content.Id, Is.EqualTo("id_2"));
    }
}
