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
        _panel.Children.AddBegin(panel1);
        _panel.Children.AddBegin(panel2);
        var result = _panel.GetAllChildren().ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0], Is.EqualTo(panel2));
        Assert.That(result[1], Is.EqualTo(panel1));
    }

    [Test]
    public void GetAllChildren2()
    {
        var panel1 = new Panel("panel_1", new(_nameGenerator.Object));
        var panel2 = new Panel("panel_2", new(_nameGenerator.Object));
        _panel.Children.AddBegin(panel1);
        panel1.Children.AddBegin(panel2);
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
}
