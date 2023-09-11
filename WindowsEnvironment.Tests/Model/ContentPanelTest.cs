using WindowsEnvironment.Model;

namespace WindowsEnvironment.Tests.Model;

internal class ContentPanelTest
{
    private Mock<INameGenerator> _nameGenerator;
    private ContentPanel _panel;

    [SetUp]
    public void Setup()
    {
        _nameGenerator = new Mock<INameGenerator>();
        _panel = new ContentPanel("panel", new(_nameGenerator.Object));
    }

    [Test]
    public void PanelState_Set()
    {
        Assert.That(_panel.State, Is.EqualTo(PanelState.Set));
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
    public void TabCollection_ParentPanel()
    {
        Assert.That(_panel.Tab.ParentPanel, Is.EqualTo(_panel));
    }
}
