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
    private IFlexWindowsEnvironment? _model;
    private IMouseController? _mouseController;
    private IFlexWindowsEnvironmentStyles? _styles;
    private PositionMarksWindow? _marksWindow;

    public void InitModel(IFlexWindowsEnvironment model)
    {
        _model = model;
    }

    public void InitMouseController(IMouseController mouseController)
    {
        _mouseController = mouseController;
    }

    public void InitStyles(IFlexWindowsEnvironmentStyles styles)
    {
        _styles = styles;
    }

    public Grid AddPanel(IPanel? parentPanel, IPanel childPanel)
    {
        var parentGrid = parentPanel != null ? this.FindChildRec<Grid>(parentPanel.Name) : this;
        if (parentPanel?.Orientation == SplitOrientation.ByRows)
        {
            parentGrid.RowDefinitions.Add(new());
        }
        else if (parentPanel?.Orientation == SplitOrientation.ByCols)
        {
            parentGrid.ColumnDefinitions.Add(new());
        }
        var childGrid = new Grid { Name = childPanel.Name };
        parentGrid.Children.Add(childGrid);

        return childGrid;
    }

    public Grid AddPanelWithTab(IPanel parentPanel, IPanel childPanel, IContentTab tab)
    {
        if (_styles == null) throw new InvalidOperationException();
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

    private void RemovePanel(RemovedPanel removedPanel)
    {
        var parentGrid = this.FindChildRec<Grid>(removedPanel.Parent.Name);
        var removedGrid = this.FindChildRec<Grid>(removedPanel.Removed.Name);
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

    public void MakeSplitters(IPanel parentPanel)
    {
        if (_styles == null) throw new InvalidOperationException();
        var parentGrid = this.FindChildRec<Grid>(parentPanel.Name);
        var needSplittersCount = parentGrid.Children.OfType<Grid>().Count() - 1;
        var currentSplittersCount = parentGrid.Children.OfType<GridSplitter>().Count();
        for (var i = currentSplittersCount; i < needSplittersCount; i++)
        {
            if (parentPanel.Orientation == SplitOrientation.ByRows)
            {
                parentGrid.RowDefinitions.Add(new());
            }
            else if (parentPanel.Orientation == SplitOrientation.ByCols)
            {
                parentGrid.ColumnDefinitions.Add(new());
            }
            parentGrid.Children.Add(GridSplitterFactory.MakeSplitter(parentPanel.Orientation, _styles.HorizontalSplitterStyle, _styles.VerticalSplitterStyle));
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
            if (parentPanel.Orientation == SplitOrientation.ByRows)
            {
                var row = parentGrid.RowDefinitions[rowColumnIndex];
                row.MinHeight = Constants.RowColMinHeight;
                row.DataContext = childPanel;
                var bind = new Binding("Size") { Converter = PanelSizeConverter.Instance };
                row.SetBinding(RowDefinition.HeightProperty, bind);
                Grid.SetRow(childGrid, rowColumnIndex);
            }
            else if (parentPanel.Orientation == SplitOrientation.ByCols)
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
            if (parentPanel.Orientation == SplitOrientation.ByRows)
            {
                var row = parentGrid.RowDefinitions[rowColumnIndex];
                row.DataContext = null;
                row.Height = new(0, GridUnitType.Auto);
                Grid.SetRow(splitter, rowColumnIndex);
            }
            else if (parentPanel.Orientation == SplitOrientation.ByCols)
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
        if (_styles == null) throw new InvalidOperationException();
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
                Style = parentPanel.IsMain ? _styles.MainPanelTabControlStyle : _styles.PanelTabControlStyle,
            };
            tabControl.SetBinding(TabControl.SelectedValueProperty, "SelectedTabName"); // bind to Panel.SelectedTabName
            parentGrid.Children.Add(tabControl);
        }
        MakeNewTab(tabControl, parentPanel, tab);
    }

    public void RemoveTab(RemovedPanel? removedPanel, IPanel tabPanel, IContentTab tab, RemoveTabMode mode)
    {
        if (_model == null) throw new InvalidOperationException();
        if (_styles == null) throw new InvalidOperationException();
        var tabPanelGrid = this.FindChildRec<Grid>(tabPanel.Name);
        var tabControl = tabPanelGrid.FindChildren<TabControl>().First();
        var tabItem = tabControl.Items.GetByName(tab.Name)!;
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
            flexWindow.HeaderBackground = _styles.FlexWindowHeaderBackground;
            flexWindow.HeaderForeground = _styles.FlexWindowHeaderForeground;
            flexWindow.HeaderMouseUp += (s, e) =>
            {
                if (_marksWindow == null) return;
                var (marks, selectedPosition) = _marksWindow.GetSelectedMarkAndPosition(this);
                if (marks != null)
                {
                    _model.SetPanelPosition(selectedPosition!.PanelName, selectedPosition.Position, flexWindow.Content!);
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
                    _marksWindow.PositionMarksBackground = _styles.PositionMarksBackground;
                    _marksWindow.HighlightedPositionBackground = _styles.HighlightedMarkPositionBackground;
                    var tabPanels = _model.AllPanels.Where(x => x.AllowTabs).ToList();
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
        var headerText = new TextBlock { DataContext = contentTab.Content.Header.SourceObject };
        headerText.SetBinding(TextBlock.TextProperty, contentTab.Content.Header.PropertyName);
        var headerGrid = new Grid();
        headerGrid.Children.Add(headerText);
        headerGrid.MouseDown += (_, me) =>
        {
            if (me.LeftButton == MouseButtonState.Pressed)
            {
                var mouse = Mouse.GetPosition(this);
                _mouseController!.OnTabHeaderButtonDown(panel.Name, contentTab.Name, mouse.X, mouse.Y);
                Mouse.Capture(headerGrid);
            }
            else if (me.MiddleButton == MouseButtonState.Pressed)
            {
                _mouseController!.OnTabHeaderMiddleButtonPress(panel.Name, contentTab.Name);
            }
        };
        headerGrid.MouseMove += (_, _) =>
        {
            var mouse = Mouse.GetPosition(this);
            _mouseController!.OnTabHeaderMouseMove(mouse.X, mouse.Y);
        };
        headerGrid.MouseUp += (_, _) =>
        {
            _mouseController!.OnTabHeaderButtonUp();
            Mouse.Capture(null);
        };
        var tabItem = new TabItem { Name = contentTab.Name, Content = contentTab.Content.View, Header = headerGrid };
        tabControl.Items.Add(tabItem);
    }

    public void SetBackgroundView(UIElement view)
    {
        var mainPanelGrid = this.FindChildRec<Grid>(MainPanel.Name);
        mainPanelGrid.Children.Insert(0, view);
    }
}
