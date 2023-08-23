using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WindowsEnvironment.Controllers;
using WindowsEnvironment.Model;
using WindowsEnvironment.Utils;

namespace WindowsEnvironment.View;

internal class MasterGrid : Grid
{
    private PositionMarksWindow? _marksWindow;

    public IFlexWindowsEnvironment? Model;
    public IMouseController? MouseController;
    public IFlexWindowsEnvironmentStyles? Styles;
    public IEnumerable<FlexWindow>? FlexWindows;

    public Grid AddPanel(IPanel? parentPanel, IPanel childPanel)
    {
        var parentGrid = parentPanel != null ? this.FindChildRec<Grid>(parentPanel.Name) : this;
        if (parentPanel?.Orientation == PanelOrientation.ByRows)
        {
            parentGrid.RowDefinitions.Add(new());
        }
        else if (parentPanel?.Orientation == PanelOrientation.ByCols)
        {
            parentGrid.ColumnDefinitions.Add(new());
        }
        var childGrid = new Grid { Name = childPanel.Name };
        parentGrid.Children.Add(childGrid);

        return childGrid;
    }

    public Grid AddPanelWithTab(IPanel parentPanel, IPanel childPanel, IContentTab tab)
    {
        if (Styles == null) throw new InvalidOperationException();
        var childGrid = AddPanel(parentPanel, childPanel);
        AddTab(childPanel, tab);
        MakeSplitters(parentPanel);
        SetPanelRowsCols(parentPanel);
        SetSplittersRowsCols(parentPanel);

        return childGrid;
    }

    public void ChangeParent(IPanel parentPanel, IPanel childPanel)
    {
        var parentGrid = new Grid { Name = parentPanel.Name };
        parentGrid.RowDefinitions.Add(new());
        parentGrid.ColumnDefinitions.Add(new());
        var childGrid = this.FindChildRec<Grid>(childPanel.Name);
        var oldParent = (Grid)childGrid.Parent;
        oldParent.Children.Remove(childGrid);
        oldParent.Children.Add(parentGrid);
        parentGrid.Children.Add(childGrid);
        Grid.SetRow(parentGrid, Grid.GetRow(childGrid));
        Grid.SetColumn(parentGrid, Grid.GetColumn(childGrid));
    }

    private void RemovePanel(RemovedPanelInfo removedPanel)
    {
        var parentGrid = this.FindChildRec<Grid>(removedPanel.Parent.Name);
        var removedGrid = this.FindChildRec<Grid>(removedPanel.Removed!.Name);
        var removedSplitter = parentGrid.FindChildren<GridSplitter>().First();
        if (removedPanel.Parent.Orientation == PanelOrientation.ByRows)
        {
            var removedGridRow = parentGrid.RowDefinitions[Grid.GetRow(removedGrid)];
            var removedSplitterRow = parentGrid.RowDefinitions[Grid.GetRow(removedSplitter)];
            parentGrid.RowDefinitions.Remove(removedGridRow);
            parentGrid.RowDefinitions.Remove(removedSplitterRow);
        }
        else if (removedPanel.Parent.Orientation == PanelOrientation.ByCols)
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

    public void MakeSplitters(IPanel parentPanel)
    {
        if (Styles == null) throw new InvalidOperationException();
        var parentGrid = this.FindChildRec<Grid>(parentPanel.Name);
        var needSplittersCount = parentGrid.Children.OfType<Grid>().Count() - 1;
        var currentSplittersCount = parentGrid.Children.OfType<GridSplitter>().Count();
        for (var i = currentSplittersCount; i < needSplittersCount; i++)
        {
            if (parentPanel.Orientation == PanelOrientation.ByRows)
            {
                parentGrid.RowDefinitions.Add(new());
            }
            else if (parentPanel.Orientation == PanelOrientation.ByCols)
            {
                parentGrid.ColumnDefinitions.Add(new());
            }
            parentGrid.Children.Add(GridSplitterFactory.MakeSplitter(parentPanel.Orientation, Styles.HorizontalSplitterStyle, Styles.VerticalSplitterStyle));
        }
    }

    public void SetPanelRowsCols(IPanel parentPanel)
    {
        var parentGrid = this.FindChildRec<Grid>(parentPanel.Name);
        for (int childPanelIndex = 0; childPanelIndex < parentPanel.Children.Count; childPanelIndex++)
        {
            var rowColumnIndex = 2 * childPanelIndex;
            var childPanel = parentPanel.Children[childPanelIndex];
            var childGrid = this.FindChildRec<Grid>(childPanel.Name);
            if (parentPanel.Orientation == PanelOrientation.ByRows)
            {
                var row = parentGrid.RowDefinitions[rowColumnIndex];
                row.MinHeight = Constants.RowColMinHeight;
                row.DataContext = childPanel;
                var bind = new Binding("Size") { Converter = PanelSizeConverter.Instance };
                row.SetBinding(RowDefinition.HeightProperty, bind);
                Grid.SetRow(childGrid, rowColumnIndex);
            }
            else if (parentPanel.Orientation == PanelOrientation.ByCols)
            {
                var col = parentGrid.ColumnDefinitions[rowColumnIndex];
                col.MinWidth = Constants.RowColMinHeight;
                col.DataContext = childPanel;
                var bind = new Binding("Size") { Converter = PanelSizeConverter.Instance };
                col.SetBinding(ColumnDefinition.WidthProperty, bind);
                Grid.SetColumn(childGrid, rowColumnIndex);
            }
        }
    }

    public void SetSplittersRowsCols(IPanel parentPanel)
    {
        var parentGrid = this.FindChildRec<Grid>(parentPanel.Name);
        var splitters = parentGrid.FindChildren<GridSplitter>().ToList();
        for (int splitterIndex = 0; splitterIndex < splitters.Count; splitterIndex++)
        {
            var splitter = splitters[splitterIndex];
            var rowColumnIndex = 2 * splitterIndex + 1;
            if (parentPanel.Orientation == PanelOrientation.ByRows)
            {
                var row = parentGrid.RowDefinitions[rowColumnIndex];
                row.DataContext = null;
                row.Height = new(0, GridUnitType.Auto);
                Grid.SetRow(splitter, rowColumnIndex);
            }
            else if (parentPanel.Orientation == PanelOrientation.ByCols)
            {
                var col = parentGrid.ColumnDefinitions[rowColumnIndex];
                col.DataContext = null;
                col.Width = new(0, GridUnitType.Auto);
                Grid.SetColumn(splitter, rowColumnIndex);
            }
        }
    }

    public void AddTab(IPanel parentPanel, IContentTab tab)
    {
        if (Styles == null) throw new InvalidOperationException();
        var parentGrid = this.FindChildRec<Grid>(parentPanel.Name);
        TabControl tabControl;
        if (parentGrid.FindChildren<TabControl>().Any())
        {
            tabControl = parentGrid.FindChildren<TabControl>().First();
        }
        else
        {
            tabControl = new TabControl
            {
                DataContext = parentPanel,
                SelectedValuePath = "Name", // bind to ContentTab.Name
                Style = parentPanel.IsMain ? Styles.MainPanelTabControlStyle : Styles.PanelTabControlStyle,
            };
            tabControl.SetBinding(TabControl.SelectedValueProperty, "SelectedTabName"); // bind to Panel.SelectedTabName
            parentGrid.Children.Add(tabControl);
        }
        MakeNewTab(tabControl, parentPanel, tab);
    }

    public void RemoveTab(RemovedPanelInfo? removedPanel, IPanel tabPanel, IContentTab tab, RemoveTabMode mode)
    {
        if (tabPanel.State == PanelState.Flex)
        {
            RemoveTabForFlexPanel(tab);
        }
        else
        {
            RemoveTabForSetPanel(removedPanel, tabPanel, tab, mode);
        }
    }

    private void RemoveTabForFlexPanel(IContentTab tab)
    {
        FlexWindows!.First(x => x.Content == tab.Content).Close();
    }

    private void RemoveTabForSetPanel(RemovedPanelInfo? removedPanel, IPanel tabPanel, IContentTab tab, RemoveTabMode mode)
    {
        if (Model == null) throw new InvalidOperationException();
        if (Styles == null) throw new InvalidOperationException();
        var tabPanelGrid = this.FindChildRec<Grid>(tabPanel.Name);
        var tabControl = tabPanelGrid.FindChildren<TabControl>().First();
        var tabItem = tabControl.Items.GetByName(tab.Name)!;
        if (tabItem.Content is IDisposable d) d.Dispose();
        tabItem.Content = null;
        tabItem.Template = null;
        tabControl.Items.Remove(tabItem);
        if (tabControl.Items.Count == 0)
        {
            tabPanelGrid.Children.Remove(tabControl);
        }
        if (removedPanel != null)
        {
            RemovePanel(removedPanel);
        }
        if (mode == RemoveTabMode.Unset)
        {
            var parentPosition = PointToScreen(new());
            var mousePosition = Mouse.GetPosition(this);
            var flexWindow = new FlexWindow(this, tab, tabControl, parentPosition, mousePosition);
            flexWindow.HeaderBackground = Styles.FlexWindowHeaderBackground;
            flexWindow.HeaderForeground = Styles.FlexWindowHeaderForeground;
            flexWindow.HeaderMouseUp += (s, e) =>
            {
                if (_marksWindow == null) return;
                var (marks, selectedPosition) = _marksWindow.GetSelectedMarkAndPosition(this);
                if (marks != null)
                {
                    flexWindow.Close();
                    Model.SetPanelPosition(selectedPosition!.PanelName, selectedPosition.Position, flexWindow.Content!);
                }
                _marksWindow.Close();
                _marksWindow = null;
            };
            flexWindow.WindowMoved += (s, e) =>
            {
                if (_marksWindow == null)
                {
                    _marksWindow = new PositionMarksWindow(this);
                    _marksWindow.PositionMarksBackground = Styles.PositionMarksBackground;
                    _marksWindow.HighlightedPositionBackground = Styles.HighlightedMarkPositionBackground;
                    var tabPanels = Model.AllPanels.Where(x => x.State == PanelState.Set && x.AllowTabs).ToList();
                    foreach (var tabPanel in tabPanels)
                    {
                        var tabPanelGrid = this.FindChildRec<Grid>(tabPanel.Name);
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

    private void MakeNewTab(TabControl tabControl, IPanel panel, IContentTab contentTab)
    {
        if (Model == null) throw new InvalidOperationException();
        if (Styles == null) throw new InvalidOperationException();
        if (MouseController == null) throw new InvalidOperationException();
        // tab header
        var headerGrid = new TabItemHeaderView
        {
            DataContext = contentTab.Content.Header.SourceObject,
            Foreground = Styles.FlexWindowHeaderForeground,
            IsCloseButtonVisible = panel.IsMain
        };
        headerGrid.SetBinding(TabItemHeaderView.HeaderTextProperty, contentTab.Content.Header.PropertyName);
        headerGrid.CloseButtonClick += (_, _) => Model.RemoveTab(contentTab.Name, RemoveTabMode.Close);
        // handlers
        void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mouse = Mouse.GetPosition(this);
                MouseController.OnTabHeaderButtonDown(contentTab.Name, mouse.X, mouse.Y);
                Mouse.Capture(headerGrid);
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                MouseController.OnTabHeaderMiddleButtonPress(contentTab.Name);
            }
        }
        void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var mouse = Mouse.GetPosition(this);
            MouseController.OnTabHeaderMouseMove(mouse.X, mouse.Y);
        }
        void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            MouseController.OnTabHeaderButtonUp();
            Mouse.Capture(null);
        }
        headerGrid.MouseDown += MouseDownHandler;
        headerGrid.MouseMove += MouseMoveHandler;
        headerGrid.MouseUp += MouseUpHandler;
        // tab content
        object tabContent;
        if (panel.IsMain)
        {
            tabContent = contentTab.Content.View;
        }
        else
        {
            var contentView = new ContentView
            {
                DataContext = contentTab.Content.Header.SourceObject,
                ContentElement = (UIElement)contentTab.Content.View,
                HeaderBackground = Styles.FlexWindowHeaderBackground, // FlexWindowHeaderBackground rename
                HeaderForeground = Styles.FlexWindowHeaderForeground  // rename
            };
            contentView.SetBinding(ContentView.HeaderTextProperty, contentTab.Content.Header.PropertyName);
            contentView.HeaderMouseDown += MouseDownHandler;
            contentView.HeaderMouseMove += MouseMoveHandler;
            contentView.HeaderMouseUp += MouseUpHandler;
            contentView.CloseButtonClick += (_, _) =>
            {
                Model.RemoveTab(contentTab.Name, RemoveTabMode.Close);
            };
            tabContent = contentView;
        }
        var tabItem = new TabItem { Name = contentTab.Name, Content = tabContent, Header = headerGrid };
        tabControl.Items.Add(tabItem);
    }

    public void SetBackgroundView(UIElement view)
    {
        var mainPanelGrid = this.FindChildRec<Grid>(MainPanel.Name);
        if (mainPanelGrid.Children.Count == 2) throw new Exception("BackgroundView has already been set.");
        mainPanelGrid.Children.Insert(0, view);
    }
}
