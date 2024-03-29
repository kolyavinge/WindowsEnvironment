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
        var masterGrid = view.MasterGrid ?? throw new InvalidOperationException();
        masterGrid.Model = model;
        masterGrid.MouseController = mouseController;
        masterGrid.FlexWindows = new FlexWindowCollection(Application.Current);
        var masterGridInitializer = new MasterGridInitializer(model.MakeReader(), masterGrid);
        masterGridInitializer.Init();
        masterGrid.SetBackgroundView(view.BackgroundView);
        model.Events.PanelAdded += (_, e) => masterGrid.AddPanelWithTab(e.ParentPanel, e.ChildPanel, e.Tab);
        model.Events.ParentChanged += (_, e) => masterGrid.ChangeParent(e.ParentPanel, e.ChildPanel);
        model.Events.TabAdded += (_, e) => masterGrid.AddTab(e.ParentPanel, e.Tab);
        model.Events.TabRemoved += (_, e) => masterGrid.RemoveTab(e.RemovedPanel, e.TabPanel, e.Tab, e.Mode);
        view.ModelInitialized?.Invoke(view, EventArgs.Empty);
    }
    #endregion

    #region BackgroundView
    public UIElement BackgroundView
    {
        get { return (UIElement)GetValue(BackgroundViewProperty); }
        set { SetValue(BackgroundViewProperty, value); }
    }

    public static readonly DependencyProperty BackgroundViewProperty =
        DependencyProperty.Register("BackgroundView", typeof(UIElement), typeof(FlexWindowsEnvironmentView), new(OnBackgroundViewChangedCallback));

    private static void OnBackgroundViewChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (FlexWindowsEnvironmentView)d;
        view.MasterGrid?.SetBackgroundView((UIElement)e.NewValue);
    }
    #endregion

    public event EventHandler? ModelInitialized;

    internal MasterGrid? MasterGrid => (MasterGrid?)Template.FindName("master", this);

    public FlexWindowsEnvironmentView()
    {
        var template = new ControlTemplate(typeof(FlexWindowsEnvironmentView));
        template.VisualTree = new FrameworkElementFactory(typeof(MasterGrid), "master");
        Template = template;
        Application.Current.MainWindow.Activated += OnMainWindowActivated;
        Application.Current.MainWindow.Deactivated += OnMainWindowDeactivated;
        Application.Current.MainWindow.StateChanged += OnWindowStateChanged;
        Application.Current.MainWindow.Closed += OnMainWindowClosed;
    }

    private void OnMainWindowActivated(object? sender, EventArgs e)
    {
        Application.Current.Windows.GetFlexWindows().Each(x => x.Topmost = true);
    }

    private void OnMainWindowDeactivated(object? sender, EventArgs e)
    {
        Application.Current.Windows.GetFlexWindows().Each(x => x.Topmost = false);
    }

    private void OnWindowStateChanged(object? sender, EventArgs e)
    {
        if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
        {
            Application.Current.Windows.GetFlexWindows().Each(x => x.Hide());
        }
        else
        {
            Application.Current.Windows.GetFlexWindows().Each(x => x.Show());
        }
    }

    private void OnMainWindowClosed(object? sender, EventArgs e)
    {
        Application.Current.Windows.GetFlexWindows().Each(x => x.Close());
    }

    public override void OnApplyTemplate()
    {
        MasterGrid!.Name = "master";
        MasterGrid.Styles = this;
    }
}
