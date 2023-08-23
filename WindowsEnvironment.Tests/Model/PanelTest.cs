using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class PanelTest
{
    private Mock<INameGenerator> _nameGenerator;
    private Panel _panel;

    [SetUp]
    public void Setup()
    {
        _nameGenerator = new Mock<INameGenerator>();
        _panel = new Panel("panel", new(_nameGenerator.Object));
    }

    [Test]
    public void Constructor()
    {
        Assert.That(_panel.Orientation, Is.EqualTo(PanelOrientation.Unspecified));
    }

    [Test]
    public void GetAllChildren_EmptyPanel()
    {
        var result = _panel.GetAllChildren().ToList();

        Assert.That(result, Has.Count.EqualTo(0));
    }

    [Test]
    public void GetAllChildren1()
    {
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        var panel2 = new Panel("panel_2", new(_nameGenerator.Object));
        _panel.ChildrenList.Add(panel1);
        _panel.ChildrenList.Add(panel2);
        var result = _panel.GetAllChildren().ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0], Is.EqualTo(panel1));
        Assert.That(result[1], Is.EqualTo(panel2));
    }

    [Test]
    public void GetAllChildren2()
    {
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        var panel2 = new Panel("panel_2", new(_nameGenerator.Object));
        _panel.ChildrenList.Add(panel1);
        panel1.ChildrenList.Add(panel2);
        var result = _panel.GetAllChildren().ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0], Is.EqualTo(panel1));
        Assert.That(result[1], Is.EqualTo(panel2));
    }

    [Test]
    public void SelectedTabNameIsNull()
    {
        Assert.That(_panel.SelectedTabName, Is.Null);
    }

    [Test]
    public void SizeIsNull()
    {
        Assert.That(_panel.Size, Is.Null);
    }

    [Test]
    public void SetOrientation_Middle_Error()
    {
        try
        {
            _panel.SetOrientation(PanelPosition.Middle);
            Assert.Fail();
        }
        catch (ArgumentException e)
        {
            Assert.That(e.Message, Is.EqualTo("Position must be Left, Right, Top or Bottom."));
        }
    }

    [Test]
    public void IsSuitableOrientation_Middle_Error()
    {
        try
        {
            _panel.IsSuitableOrientation(PanelPosition.Middle);
            Assert.Fail();
        }
        catch (ArgumentException e)
        {
            Assert.That(e.Message, Is.EqualTo("Position must be Left, Right, Top or Bottom."));
        }
    }

    [Test]
    public void IsSuitableOrientation_PanelOrientationUnspecified()
    {
        Assert.True(_panel.IsSuitableOrientation(PanelPosition.Left));
        Assert.True(_panel.IsSuitableOrientation(PanelPosition.Right));
        Assert.True(_panel.IsSuitableOrientation(PanelPosition.Top));
        Assert.True(_panel.IsSuitableOrientation(PanelPosition.Bottom));
    }

    [Test]
    public void IsSuitableOrientation_PanelOrientationByCols()
    {
        _panel.SetOrientation(PanelPosition.Left);

        Assert.True(_panel.IsSuitableOrientation(PanelPosition.Left));
        Assert.True(_panel.IsSuitableOrientation(PanelPosition.Right));
        Assert.False(_panel.IsSuitableOrientation(PanelPosition.Top));
        Assert.False(_panel.IsSuitableOrientation(PanelPosition.Bottom));
    }

    [Test]
    public void IsSuitableOrientation_PanelOrientationByRows()
    {
        _panel.SetOrientation(PanelPosition.Top);

        Assert.False(_panel.IsSuitableOrientation(PanelPosition.Left));
        Assert.False(_panel.IsSuitableOrientation(PanelPosition.Right));
        Assert.True(_panel.IsSuitableOrientation(PanelPosition.Top));
        Assert.True(_panel.IsSuitableOrientation(PanelPosition.Bottom));
    }
}
