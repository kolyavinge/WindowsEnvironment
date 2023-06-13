using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsEnvironment.Controllers;
using WindowsEnvironment.Model;
using WindowsEnvironment.Utils;

namespace WindowsEnvironment.View;

public class FlexWindowsEnvironmentView : Control
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
        var control = (FlexWindowsEnvironmentView)d;
        var model = (IFlexWindowsEnvironment)e.NewValue;
        model.Events.PanelAdded += control.OnPanelAdded;
        model.Events.ParentChanged += control.OnParentChanged;
        model.Events.TabAdded += control.OnTabAdded;
        model.Events.TabRemoved += control.OnTabRemoved;
        control._mouseController = MouseControllerFactory.Make(model);
        control.ModelInitialized?.Invoke(control, EventArgs.Empty);
    }
    #endregion

    private IMouseController? _mouseController;
    private Grid _masterGrid;
    private PositionMarksWindow? _marksWindow;

    public event EventHandler? ModelInitialized;

    public FlexWindowsEnvironmentView()
    {
        _masterGrid = new Grid();
        var template = new ControlTemplate(typeof(FlexWindowsEnvironmentView));
        template.VisualTree = new FrameworkElementFactory(typeof(Grid), "master");
        Template = template;
        Application.Current.MainWindow.StateChanged += OnWindowStateChanged;
        Application.Current.MainWindow.MouseMove += OnWindowMouseMove;
        Application.Current.MainWindow.MouseUp += OnWindowMouseUp;
        Application.Current.MainWindow.Closed += OnWindowClosed;
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

    private void OnWindowMouseMove(object sender, MouseEventArgs e)
    {
        var mousePosition = Mouse.GetPosition(this);
        _mouseController!.OnWindowMouseMove(mousePosition.X, mousePosition.Y);
    }

    private void OnWindowMouseUp(object sender, MouseButtonEventArgs e)
    {
        _mouseController!.OnWindowButtonUp();
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        Application.Current.Windows.EachFlexWindow(x => x.Close());
    }

    public override void OnApplyTemplate()
    {
        _masterGrid = (Grid)Template.FindName("master", this);
        _masterGrid.Name = "master";
        var rootGrid = new Grid();
        rootGrid.Name = WindowsEnvironment.Model.Panel.MainPanelName;
        rootGrid.RowDefinitions.Add(new());
        rootGrid.ColumnDefinitions.Add(new());
        rootGrid.Children.Add(new TabControl());
        _masterGrid.Children.Add(rootGrid);
    }

    private void OnPanelAdded(object? sender, PanelAddedEventArgs e)
    {
        var parentGrid = _masterGrid.FindChild<Grid>(e.ParentPanel.Name);
        if (e.ParentPanel.Orientation == SplitOrientation.ByRows)
        {
            parentGrid.RowDefinitions.Add(new());
        }
        else if (e.ParentPanel.Orientation == SplitOrientation.ByCols)
        {
            parentGrid.ColumnDefinitions.Add(new());
        }
        var childGrid = new Grid { Name = e.ChildPanel.Name };
        childGrid.RowDefinitions.Add(new());
        childGrid.ColumnDefinitions.Add(new());
        var tabControl = new TabControl();
        AddNewTab(tabControl, e.ChildPanel.Name, e.Tab);
        childGrid.Children.Add(tabControl);
        parentGrid.Children.Add(childGrid);
        SetPanelRowsAndCols(e.ParentPanel);
    }

    private void OnParentChanged(object? sender, ParentChangedEventArgs e)
    {
        var parentGrid = new Grid { Name = e.ParentPanel.Name };
        parentGrid.RowDefinitions.Add(new());
        parentGrid.ColumnDefinitions.Add(new());
        var childGrid = _masterGrid.FindChild<Grid>(e.ChildPanel.Name);
        var oldParent = (Grid)childGrid.Parent;
        oldParent.Children.Remove(childGrid);
        oldParent.Children.Add(parentGrid);
        parentGrid.Children.Add(childGrid);
        Grid.SetRow(parentGrid, Grid.GetRow(childGrid));
        Grid.SetColumn(parentGrid, Grid.GetColumn(childGrid));
    }

    private void OnTabAdded(object? sender, TabAddedEventArgs e)
    {
        var parentGrid = _masterGrid.FindChild<Grid>(e.ParentPanel.Name);
        var tabControl = (TabControl)parentGrid.Children[0];
        AddNewTab(tabControl, e.ParentPanel.Name, e.Tab);
    }

    private void OnPanelRemoved(RemovedPanel removedPanel)
    {
        var parentGrid = _masterGrid.FindChild<Grid>(removedPanel.Parent.Name);
        var removedGrid = _masterGrid.FindChild<Grid>(removedPanel.Removed.Name);
        if (removedPanel.Parent.Orientation == SplitOrientation.ByRows)
        {
            int rowIndex = Grid.GetRow(removedGrid);
            parentGrid.RowDefinitions.RemoveAt(rowIndex);
        }
        else if (removedPanel.Parent.Orientation == SplitOrientation.ByCols)
        {
            int colIndex = Grid.GetColumn(removedGrid);
            parentGrid.ColumnDefinitions.RemoveAt(colIndex);
        }
        parentGrid.Children.RemoveByName(removedGrid.Name);
        SetPanelRowsAndCols(removedPanel.Parent);
    }

    private void OnTabRemoved(object? sender, TabRemovedEventArgs e)
    {
        var tabPanelGrid = _masterGrid.FindChild<Grid>(e.TabPanel.Name);
        var tabControl = (TabControl)tabPanelGrid.Children[0];
        var tab = tabControl.Items.GetByName(e.Tab.Name)!;
        tab.Content = null;
        tabControl.Items.Remove(tab);
        if (e.RemovedPanel != null)
        {
            OnPanelRemoved(e.RemovedPanel);
        }
        if (e.Mode == RemoveTabMode.Unset)
        {
            var parentPosition = PointToScreen(new());
            var mousePosition = Mouse.GetPosition(this);
            var flexWindow = new FlexWindow(this, e.Tab, tabControl, parentPosition, mousePosition);
            flexWindow.HeaderMouseUp += (s, e) =>
            {
                if (_marksWindow == null) return;
                var (marks, selectedPosition) = _marksWindow.GetSelectedMarkAndPosition(this);
                if (marks != null)
                {
                    Model.SetPanelPosition(selectedPosition!.PanelName, selectedPosition.Position, flexWindow.MainContent);
                    flexWindow.Close();
                }
                _marksWindow.Close();
                _marksWindow = null;
            };
            flexWindow.WindowMoved += (s, e) =>
            {
                if (_marksWindow == null)
                {
                    _marksWindow = new PositionMarksWindow(this);
                    var tabPanels = Model.AllPanels.Where(x => x.AllowTabs).ToList();
                    foreach (var tabPanel in tabPanels)
                    {
                        var tabPanelGrid = _masterGrid.FindChild<Grid>(tabPanel.Name);
                        var tabPanelGridPosition = tabPanelGrid.GetPositionWithinParent(this);
                        _marksWindow.AddMarksFor(tabPanel.Name, tabPanelGrid, tabPanelGridPosition);
                    }
                    _marksWindow.Show();
                }
                else
                {
                    _marksWindow.Topmost = false;
                    _marksWindow.Topmost = true;
                    _marksWindow.DeactivateAllPositions();
                    var (marks, selectedPosition) = _marksWindow.GetSelectedMarkAndPosition(this);
                    if (marks != null)
                    {
                        marks.ActivatePosition(selectedPosition!.Position);
                    }
                }
            };
            flexWindow.CaptureHeader(mousePosition);
            flexWindow.Show();
        }
    }

    private void SetPanelRowsAndCols(Model.Panel parentPanel)
    {
        foreach (var childPanel in parentPanel.Children)
        {
            var index = Model.GetChildPanelIndex(parentPanel.Name, childPanel.Name);
            var childGrid = _masterGrid.FindChild<Grid>(childPanel.Name);
            if (parentPanel.Orientation == SplitOrientation.ByRows)
            {
                Grid.SetRow(childGrid, index);
            }
            else if (parentPanel.Orientation == SplitOrientation.ByCols)
            {
                Grid.SetColumn(childGrid, index);
            }
        }
    }

    private void AddNewTab(TabControl tabControl, string panelName, ContentTab contentTab)
    {
        var header = new Grid();
        header.Children.Add(new TextBlock { Text = contentTab.Name });
        header.MouseDown += (_, me) =>
        {
            if (me.LeftButton == MouseButtonState.Pressed)
            {
                var mouse = Mouse.GetPosition(this);
                _mouseController!.OnTabButtonDown(panelName, contentTab.Name, mouse.X, mouse.Y);
            }
            else if (me.MiddleButton == MouseButtonState.Pressed)
            {
                _mouseController!.OnTabMiddleButtonPress(panelName, contentTab.Name);
            }
        };
        var tabItem = new TabItem { Name = contentTab.Name, Content = contentTab.Content, Header = header };
        tabControl.Items.Add(tabItem);
        tabControl.SelectedItem = tabItem;
    }
}
