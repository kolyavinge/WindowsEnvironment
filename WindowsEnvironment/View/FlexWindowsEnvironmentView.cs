using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsEnvironment.Controllers;
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
        model.Events.PanelAdded += view.OnPanelAdded;
        model.Events.ParentChanged += view.OnParentChanged;
        model.Events.TabAdded += view.OnTabAdded;
        model.Events.TabRemoved += view.OnTabRemoved;
        view._mouseController = MouseControllerFactory.Make(model);
        view.ModelInitialized?.Invoke(view, EventArgs.Empty);
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
        _masterGrid = (Grid)Template.FindName("master", this);
        _masterGrid.Name = "master";
        var rootGrid = new Grid();
        rootGrid.Name = WindowsEnvironment.Model.Panel.MainPanelName;
        rootGrid.RowDefinitions.Add(new());
        rootGrid.ColumnDefinitions.Add(new());
        rootGrid.Children.Add(new TabControl { Style = MainPanelTabControlStyle });
        _masterGrid.Children.Add(rootGrid);
    }

    private void OnPanelAdded(object? sender, PanelAddedEventArgs e)
    {
        var parentGrid = _masterGrid.FindChildRec<Grid>(e.ParentPanel.Name);
        if (e.ParentPanel.Orientation == SplitOrientation.ByRows)
        {
            parentGrid.RowDefinitions.Add(new());
            parentGrid.RowDefinitions.Add(new());
        }
        else if (e.ParentPanel.Orientation == SplitOrientation.ByCols)
        {
            parentGrid.ColumnDefinitions.Add(new());
            parentGrid.ColumnDefinitions.Add(new());
        }
        var childGrid = new Grid { Name = e.ChildPanel.Name };
        childGrid.RowDefinitions.Add(new());
        childGrid.ColumnDefinitions.Add(new());
        var tabControl = new TabControl { Style = PanelTabControlStyle };
        MakeNewTab(tabControl, e.ChildPanel.Name, e.Tab);
        childGrid.Children.Add(tabControl);
        parentGrid.Children.Add(childGrid);
        parentGrid.Children.Add(GridSplitterFactory.MakeSplitter(e.ParentPanel.Orientation, HorizontalSplitterStyle, VerticalSplitterStyle));
        SetPanelRowsCols(e.ParentPanel);
        SetSplittersRowsCols(e.ParentPanel);
    }

    private void OnParentChanged(object? sender, ParentChangedEventArgs e)
    {
        var parentGrid = new Grid { Name = e.ParentPanel.Name };
        parentGrid.RowDefinitions.Add(new());
        parentGrid.ColumnDefinitions.Add(new());
        var childGrid = _masterGrid.FindChildRec<Grid>(e.ChildPanel.Name);
        var oldParent = (Grid)childGrid.Parent;
        oldParent.Children.Remove(childGrid);
        oldParent.Children.Add(parentGrid);
        parentGrid.Children.Add(childGrid);
        Grid.SetRow(parentGrid, Grid.GetRow(childGrid));
        Grid.SetColumn(parentGrid, Grid.GetColumn(childGrid));
    }

    private void OnPanelRemoved(RemovedPanel removedPanel)
    {
        var parentGrid = _masterGrid.FindChildRec<Grid>(removedPanel.Parent.Name);
        var removedGrid = _masterGrid.FindChildRec<Grid>(removedPanel.Removed.Name);
        var removedSplitter = parentGrid.FindChildren<GridSplitter>().First();
        if (removedPanel.Parent.Orientation == SplitOrientation.ByRows)
        {
            var removedGridRow = parentGrid.RowDefinitions[Grid.GetRow(removedGrid)];
            var removedSplitterRow = parentGrid.RowDefinitions[Grid.GetRow(removedSplitter)];
            parentGrid.RowDefinitions.Remove(removedGridRow);
            parentGrid.RowDefinitions.Remove(removedSplitterRow);
        }
        else if (removedPanel.Parent.Orientation == SplitOrientation.ByCols)
        {
            var removedGridCol = parentGrid.ColumnDefinitions[Grid.GetColumn(removedGrid)];
            var removedSplitterCol = parentGrid.ColumnDefinitions[Grid.GetColumn(removedSplitter)];
            parentGrid.ColumnDefinitions.Remove(removedGridCol);
            parentGrid.ColumnDefinitions.Remove(removedSplitterCol);
        }
        parentGrid.Children.Remove(removedGrid);
        parentGrid.Children.Remove(removedSplitter);
        SetPanelRowsCols(removedPanel.Parent);
        SetSplittersRowsCols(removedPanel.Parent);
    }

    private void SetPanelRowsCols(Model.Panel parentPanel)
    {
        var parentGrid = _masterGrid.FindChildRec<Grid>(parentPanel.Name);
        for (int childPanelIndex = 0; childPanelIndex < parentPanel.Children.Count; childPanelIndex++)
        {
            var rowColumnIndex = 2 * childPanelIndex;
            var childPanel = parentPanel.Children[childPanelIndex];
            var childGrid = _masterGrid.FindChildRec<Grid>(childPanel.Name);
            if (parentPanel.Orientation == SplitOrientation.ByRows)
            {
                var row = parentGrid.RowDefinitions[rowColumnIndex];
                row.MinHeight = Constants.RowColMinHeight;
                Grid.SetRow(childGrid, rowColumnIndex);
            }
            else if (parentPanel.Orientation == SplitOrientation.ByCols)
            {
                var col = parentGrid.ColumnDefinitions[rowColumnIndex];
                col.MinWidth = Constants.RowColMinHeight;
                Grid.SetColumn(childGrid, rowColumnIndex);
            }
        }
    }

    private void SetSplittersRowsCols(Model.Panel parentPanel)
    {
        var parentGrid = _masterGrid.FindChildRec<Grid>(parentPanel.Name);
        var splitters = parentGrid.FindChildren<GridSplitter>().ToList();
        for (int splitterIndex = 0; splitterIndex < splitters.Count; splitterIndex++)
        {
            var splitter = splitters[splitterIndex];
            var rowColumnIndex = 2 * splitterIndex + 1;
            if (parentPanel.Orientation == SplitOrientation.ByRows)
            {
                var row = parentGrid.RowDefinitions[rowColumnIndex];
                row.Height = new(0, GridUnitType.Auto);
                Grid.SetRow(splitter, rowColumnIndex);
            }
            else if (parentPanel.Orientation == SplitOrientation.ByCols)
            {
                var col = parentGrid.ColumnDefinitions[rowColumnIndex];
                col.Width = new(0, GridUnitType.Auto);
                Grid.SetColumn(splitter, rowColumnIndex);
            }
        }
    }

    private void OnTabAdded(object? sender, TabAddedEventArgs e)
    {
        var parentGrid = _masterGrid.FindChildRec<Grid>(e.ParentPanel.Name);
        var tabControl = (TabControl)parentGrid.Children[0];
        MakeNewTab(tabControl, e.ParentPanel.Name, e.Tab);
    }

    private void OnTabRemoved(object? sender, TabRemovedEventArgs e)
    {
        var tabPanelGrid = _masterGrid.FindChildRec<Grid>(e.TabPanel.Name);
        var tabControl = (TabControl)tabPanelGrid.Children[0];
        var tab = tabControl.Items.GetByName(e.Tab.Name)!;
        tab.Content = null;
        tab.Template = null;
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
            flexWindow.HeaderBackground = FlexWindowHeaderBackground;
            flexWindow.HeaderForeground = FlexWindowHeaderForeground;
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
                    _marksWindow.PositionMarksBackground = PositionMarksBackground;
                    _marksWindow.HighlightedPositionBackground = HighlightedMarkPositionBackground;
                    var tabPanels = Model.AllPanels.Where(x => x.AllowTabs).ToList();
                    foreach (var tabPanel in tabPanels)
                    {
                        var tabPanelGrid = _masterGrid.FindChildRec<Grid>(tabPanel.Name);
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

    private void MakeNewTab(TabControl tabControl, string panelName, ContentTab contentTab)
    {
        var header = new Grid();
        header.Children.Add(new TextBlock { Text = contentTab.Name });
        header.MouseDown += (_, me) =>
        {
            if (me.LeftButton == MouseButtonState.Pressed)
            {
                var mouse = Mouse.GetPosition(this);
                _mouseController!.OnTabHeaderButtonDown(panelName, contentTab.Name, mouse.X, mouse.Y);
                Mouse.Capture(header);
            }
            else if (me.MiddleButton == MouseButtonState.Pressed)
            {
                _mouseController!.OnTabHeaderMiddleButtonPress(panelName, contentTab.Name);
            }
        };
        header.MouseMove += (_, _) =>
        {
            var mouse = Mouse.GetPosition(this);
            _mouseController!.OnTabHeaderMouseMove(mouse.X, mouse.Y);
        };
        header.MouseUp += (_, _) =>
        {
            _mouseController!.OnTabHeaderButtonUp();
            Mouse.Capture(null);
        };
        var tabItem = new TabItem { Name = contentTab.Name, Content = contentTab.Content, Header = header };
        tabControl.Items.Add(tabItem);
        tabControl.SelectedItem = tabItem;
    }
}
