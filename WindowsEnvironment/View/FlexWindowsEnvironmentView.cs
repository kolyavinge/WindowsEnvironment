using System.Windows;
using System.Windows.Controls;
using WindowsEnvironment.Model;
using WindowsEnvironment.Utils;

namespace WindowsEnvironment.View;

public partial class FlexWindowsEnvironmentView : Control
{
    #region Model
    public IFlexWindowsEnvironment Model
    {
        get { return (IFlexWindowsEnvironment)GetValue(ModelProperty); }
        set { SetValue(ModelProperty, value); }
    }

    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(IFlexWindowsEnvironment), typeof(FlexWindowsEnvironmentView), new(OnModelChangedCallback));

    private static void OnModelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (FlexWindowsEnvironmentView)d;
        var model = (IFlexWindowsEnvironment)e.NewValue;
        var mouseController = MouseControllerFactory.Make(model);
        var masterGrid = view._masterGrid ?? throw new InvalidOperationException();
        masterGrid.InitModel(model);
        masterGrid.InitMouseController(mouseController);
        model.Events.PanelAdded += (_, e) => masterGrid.AddPanel(e.ParentPanel, e.ChildPanel, e.Tab);
        model.Events.ParentChanged += (_, e) => masterGrid.ChangeParent(e.ParentPanel, e.ChildPanel);
        model.Events.TabAdded += (_, e) => masterGrid.AddTab(e.ParentPanel, e.Tab);
        model.Events.TabRemoved += (_, e) => masterGrid.RemoveTab(e.RemovedPanel, e.TabPanel, e.Tab, e.Mode);
        view.ModelInitialized?.Invoke(view, EventArgs.Empty);
    }
    #endregion

    private MasterGrid? _masterGrid;

    public event EventHandler? ModelInitialized;

    public FlexWindowsEnvironmentView()
    {
        var template = new ControlTemplate(typeof(FlexWindowsEnvironmentView));
        template.VisualTree = new FrameworkElementFactory(typeof(MasterGrid), "master");
        Template = template;
        Application.Current.MainWindow.StateChanged += OnWindowStateChanged;
        Application.Current.MainWindow.Closed += OnMainWindowClosed;
    }

    private void OnWindowStateChanged(object? sender, EventArgs e)
    {
        if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
        {
            Application.Current.Windows.EachFlexWindow(x => x.Hide());
        }
        else
        {
            Application.Current.Windows.EachFlexWindow(x => x.Show());
        }
    }

    private void OnMainWindowClosed(object? sender, EventArgs e)
    {
        Application.Current.Windows.EachFlexWindow(x => x.Close());
    }

    public override void OnApplyTemplate()
    {
        _masterGrid = (MasterGrid)Template.FindName("master", this);
        _masterGrid.Name = "master";
        _masterGrid.InitStyles(this);
        _masterGrid.MakeRootGrid();
    }
}
