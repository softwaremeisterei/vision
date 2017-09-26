using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Docking.Controls
{
    /// <summary>
    /// Container for auto-dockable tool windows. Place this on your form to allow auto-docking for the tool windows.
    /// </summary>
    public partial class DockContainer : UserControl
    {
        private const int HooverMouseDelta = 10;

        private DockPanelsResizer _panels = null;
        private DockPreviewEngine _dockPreviewEngine = new DockPreviewEngine();

        private List<ToolWindow> _undockedToolWindows = new List<ToolWindow>();
        private List<ToolWindow> _dockableToolWindows = new List<ToolWindow>();
        private ToolWindow _movedToolWindow = null;
        private List<TabButton> _tabButtons = new List<TabButton>();

        private bool _closeCenterButtonHoover = false;
        private bool _menuCenterButtonHoover = false;
        private bool _undockCenterButtonHoover = false;

        private Color _tabButtonSelectedBackColor2 = Color.FromArgb(255, 215, 157);
        private Color _tabButtonSelectedBackColor1 = Color.FromArgb(255, 242, 200);
        private Color _tabButtonSelectedColor = Color.Black;
        private Color _tabButtonSelectedBorderColor = Color.FromArgb(75, 75, 111);
        private Color _tabButtonNotSelectedColor = Color.DarkGray;
        private bool _tabButtonShowSelection = false;

        private Point _lastMouseLocation = new Point();
        private bool _selectToolWindowsOnHoover = false;

        /// <summary>
        /// Default constructor which creates a new instance of <see cref="DockContainer"/>
        /// </summary>
        public DockContainer()
        {
            InitializeComponent();

            _panels = new DockPanelsResizer(this);

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            BorderStyle = BorderStyle.None;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;

            _dockPreviewEngine.VisibleChanged += OnDockPreviewVisibleChanged;
            _mouseCheckTimer.Tick += OnTestMouseState;
            _panels.MinimumSizeChanged += OnMinimumSizeChanged;
        }


        public List<ToolWindow> GetToolWindows()
        {
            return _dockableToolWindows;
        }


        /// <summary>
        /// Color of the tab button text when the button is not selected.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the tab button text when the button is not selected.")]
        public Color TabButtonNotSelectedColor
        {
            get { return _tabButtonNotSelectedColor; }
            set
            {
                _tabButtonNotSelectedColor = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.NotSelectedColor = value;
                }
            }
        }

        /// <summary>
        /// Color of the tab button text when the button is selected.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the tab button text when the button is selected.")]
        public Color TabButtonSelectedColor
        {
            get { return _tabButtonSelectedColor; }
            set
            {
                _tabButtonSelectedColor = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.SelectedColor = value;
                }
            }
        }

        /// <summary>
        /// Selected border color of the tab button.
        /// </summary>
        [Category("Appearance")]
        [Description("Border color of the tab button when is selected.")]
        public Color TabButtonSelectedBorderColor
        {
            get { return _tabButtonSelectedBorderColor; }
            set
            {
                _tabButtonSelectedBorderColor = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.SelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// First gradient color of the text when the button is selected.
        /// </summary>
        [Category("Appearance")]
        [Description("First gradient color of the tab button when is selected.")]
        public Color TabButtonSelectedBackColor1
        {
            get { return _tabButtonSelectedBackColor1; }
            set
            {
                _tabButtonSelectedBackColor1 = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.SelectedBackColor1 = value;
                }
            }
        }

        /// <summary>
        /// Second gradient color of the text when the button is selected.
        /// </summary>
        [Category("Appearance")]
        [Description("Second gradient color of the tab button when is selected.")]
        public Color TabButtonSelectedBackColor2
        {
            get { return _tabButtonSelectedBackColor2; }
            set
            {
                _tabButtonSelectedBackColor2 = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.SelectedBackColor2 = value;
                }
            }
        }

        /// <summary>
        /// Flag indicating if the button should draw border and gradient background when is selected. Default false.
        /// </summary>
        [Category("Behavior")]
        [Description("Flag indicating if the button should draw border and gradient background when is selected. Default false.")]
        public bool TabButtonShowSelection
        {
            get { return _tabButtonShowSelection; }
            set
            {
                _tabButtonShowSelection = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.ShowSelection = value;
                }
            }
        }


        /// <summary>
        /// This event occurs when the minimum allowed size for the container was changed.
        /// The form on which this container is placed should be sized to display the entire container
        /// </summary>
        /// <example>
        /// Here is a sample of how this event should be handled. It assumes that _dockContainer1 is 
        /// docked on the form with DockStyle.Fill.
        /// <para></para>
        /// In this example is computed the difference between the dock container width and height and then add these differences
        /// to the minimum size of the form. In this way, when the form has minimum size, the container will
        /// have ensured its required minimum size:
        /// <code>
        /// private void OnDockContainerMinSizeChanged (object sender, EventArgs e)
        /// {
        ///    int deltaX = Width  - _dockContainer1.Width;
        ///    int deltaY = Height - _dockContainer1.Height;
        /// 
        ///    MinimumSize = new Size (
        ///       _dockContainer1.MinimumSize.Width  + deltaX,
        ///       _dockContainer1.MinimumSize.Height + deltaY);
        /// }
        /// </code>
        /// </example>
        [Category("Layout")]
        [Description("Occurs when the minimum size of the container is changed. Use this to set the minimum size of the form.")]
        public event EventHandler MinimumSizeChanged;

        /// <summary>
        /// Event raised when context menu request was made.
        /// </summary>
        /// <example>
        /// Here is an example of how to handle this event to display a context menu for <c>Fill</c> panel and 
        /// to select a docked tool window from that panel:
        /// <code>
        /// private void OnToolWindowContextMenuRequest (object sender, ContextMenuEventArg e)
        /// {
        ///    if (e.Selection.DockMode == zDockMode.Fill &amp;&amp; e.MouseButtons == MouseButtons.Left)
        ///    {
        ///       ClearSelectWindowsMenu (_centerContextMenu.Items);
        /// 
        ///       DockableToolWindow[] panelWindows = _dockContainer.GetVisibleDockedWindows (e.Selection.DockMode);
        ///       foreach (DockableToolWindow panelWindow in panelWindows)
        ///       {
        ///          AddSelectWindowMenu (_centerContextMenu.Items, panelWindow);
        ///       }
        /// 
        ///       e.Selection.ContextMenuStrip = _centerContextMenu;
        /// 
        ///       e.Selection.ContextMenuStrip.Show (e.MouseLocation);
        ///    }
        /// }
        /// 
        /// private void AddSelectWindowMenu (ToolStripItemCollection items, DockableToolWindow window)
        /// {
        ///    ToolStripMenuItem item = new ToolStripMenuItem (window.Text);
        ///    items.Add (item);
        /// 
        ///    item.Tag = window;
        ///    item.Click += OnSelectToolWindowByMenu;
        /// }
        /// 
        /// private void ClearSelectWindowsMenu (ToolStripItemCollection items)
        /// {
        ///    foreach (ToolStripMenuItem item in items)
        ///    {
        ///       item.Tag = null;
        ///       item.Click -= OnSelectToolWindowByMenu;
        ///    }
        /// 
        ///    items.Clear ();
        /// }
        /// 
        /// private void OnSelectToolWindowByMenu (object sender, EventArgs e)
        /// {
        ///    ToolStripMenuItem item = sender as ToolStripMenuItem;
        ///    if (item == null)
        ///    {
        ///       return;
        ///    }
        /// 
        ///    DockableToolWindow window = item.Tag as DockableToolWindow;
        ///    if (window == null)
        ///    {
        ///       return;
        ///    }
        /// 
        ///    _dockContainer.SelectToolWindow (window);
        /// }
        /// </code>
        /// </example>
        [Category("Action")]
        [Description("Occurs when the context menu should be shown for the selected tool window.")]
        public event EventHandler<ContextMenuEventArg> ContextMenuRequest;

        /// <summary>
        /// Event raised when auto-hide was toggled for a panel
        /// </summary>
        [Category("Action")]
        [Description("Occurs when the auto-hide state was changed for the selected panel.")]
        public event EventHandler<AutoHideEventArgs> AutoHidePanelToggled;

        /// <summary>
        /// Occurs when a tool window was selected
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a tool window was selected.")]
        public event EventHandler<ToolSelectionChangedEventArgs> ToolWindowSelected;

        /// <summary>
        /// Flag indicating if the tool windows should be selected on hoover over their tab button when the panel
        /// is in auto-hide mode.
        /// </summary>
        [Category("Behavior")]
        [Description("Flag indicating if the tool windows should be selected on hoover over their tab button when the panel is in auto-hide mode.")]
        public bool SelectToolWindowsOnHoover
        {
            get { return _selectToolWindowsOnHoover; }
            set { _selectToolWindowsOnHoover = value; }
        }

        /// <summary>
        /// Get / set the width of the left panel
        /// </summary>
        [Category("Layout")]
        [Description("Get/set the width of the left panel.")]
        public int LeftPanelWidth
        {
            get { return _panels.LeftPanelWidth; }
            set { _panels.LeftPanelWidth = value; }
        }

        /// <summary>
        /// Get / set the width of the right panel
        /// </summary>
        [Category("Layout")]
        [Description("Get/set the width of the right panel.")]
        public int RightPanelWidth
        {
            get { return _panels.RightPanelWidth; }
            set { _panels.RightPanelWidth = value; }
        }

        /// <summary>
        /// Get / set the height of the top panel
        /// </summary>
        [Category("Layout")]
        [Description("Get/set the width of the top panel.")]
        public int TopPanelHeight
        {
            get { return _panels.TopPanelHeight; }
            set { _panels.TopPanelHeight = value; }
        }

        /// <summary>
        /// Get / set the height of the bottom panel
        /// </summary>
        [Category("Layout")]
        [Description("Get/set the width of the bottom panel.")]
        public int BottomPanelHeight
        {
            get { return _panels.BottomPanelHeight; }
            set { _panels.BottomPanelHeight = value; }
        }

        /// <summary>
        /// Checks if a tool window is added in this container
        /// </summary>
        /// <param name="toolWindow">tool window to be checked</param>
        /// <returns>true if the given tool window is added in this container</returns>
        public bool IsInContainer(ToolWindow toolWindow)
        {
            return _dockableToolWindows.Contains(toolWindow);
        }

        /// <summary>
        /// Add a tool window to be managed by this dock container. 
        /// The caller must show the form to make it visible.
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A NullReferenceException will be thrown if the tool window is null.
        /// <br/><br/>
        /// The window is not docked when is added with this method. To add the form with initial dock, use
        /// the <see cref="DockToolWindow">DockToolWindow</see> method.
        /// <br/><br/>
        /// The following properties are forced to restricted values:
        /// <ul>
        /// <li> Dock       is forced to <c>DockStyle.None</c> (an exception will be thrown if this value is changed outside of this container).</li>
        /// <li> Parent     is forced to <c>this</c> (changing it outside of the instance will cause the removal of the tool window).</li>
        /// <li> TopLevel   is forced <c>false</c> (an exception will be thrown if this value is changed outside of this container).</li>
        /// </ul>
        /// <para></para>
        /// <br/>
        /// <para></para>
        /// DockContainer is not the owner of the added toolWindow so is not responsable with
        /// disposing it.
        /// <para></para>
        /// <br/>
        /// <para></para>
        /// When a toolWindow (which was added to this container) is disposed or when its parent is 
        /// changed, that toolWindow is automatically removed from this container.
        /// </remarks>
        /// <example>
        /// Here is a sample of how to use this method. It will add a floating form to the dock container.
        /// <code>
        /// private void ShowNewForm ()
        /// {
        ///    // Create a new instance of the child form.
        ///    DockableToolWindow childForm = new DockableToolWindow ();
        /// 
        ///    // Add the form to the dock container
        ///    _dockContainer1.AddToolWindow (childForm);
        /// 
        ///    // Show the form
        ///    childForm.Show ();
        /// }
        /// </code>
        /// </example>
        /// <param name="toolWindow">tool window to be added</param>
        public void AddToolWindow(ToolWindow toolWindow)
        {
            if (IsInContainer(toolWindow))
            {
                return;
            }

            toolWindow.Move += OnToolWindowMove;
            toolWindow.Disposed += OnDisposeToolWindow;
            toolWindow.ParentChanged += OnToolWindowParentChanged;
            toolWindow.FormClosed += OnToolWindowClose;
            toolWindow.DockChanged += OnToolWindowDockChanged;
            toolWindow.AutoHideButtonClick += OnToolWindowAutoHideChanged;
            toolWindow.ContextMenuForToolWindow += OnToolWindowContextMenuRequest;

            toolWindow.Dock = DockStyle.None;
            toolWindow.TopLevel = false;
            toolWindow.Parent = this;

            _dockableToolWindows.Add(toolWindow);
            _undockedToolWindows.Add(toolWindow);

            CreateTabButton(toolWindow);

            SelectToolWindow(toolWindow);

            MoveInFront(toolWindow);
        }

        /// <summary>
        /// Removes a tool window from this dock container.
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A <exception cref="NullReferenceException">NullReferenceException</exception> 
        /// will be thrown if the tool window is null.
        /// <br/>
        /// </remarks>
        /// <param name="toolWindow">tool window to be removed</param>
        public void RemoveToolWindow(ToolWindow toolWindow)
        {
            toolWindow.Dead = true;

            if (toolWindow.Parent == this)
            {
                toolWindow.Parent = null;
            }

            toolWindow.Move -= OnToolWindowMove;
            toolWindow.Disposed -= OnDisposeToolWindow;
            toolWindow.ParentChanged -= OnToolWindowParentChanged;
            toolWindow.FormClosed -= OnToolWindowClose;
            toolWindow.DockChanged -= OnToolWindowDockChanged;
            toolWindow.AutoHideButtonClick -= OnToolWindowAutoHideChanged;
            toolWindow.ContextMenuForToolWindow -= OnToolWindowContextMenuRequest;

            _dockableToolWindows.Remove(toolWindow);
            _undockedToolWindows.Remove(toolWindow);

            _panels.UndockToolWindow(toolWindow);

            RemoveTabButton(toolWindow);
        }

        /// <summary>
        /// Dock the given tool window inside the dock container.
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A <exception cref="NullReferenceException">NullReferenceException</exception> 
        /// will be thrown if the tool window is null.
        /// <br/>
        /// If the dock mode is None or is not Left, Right, Top, Bottom or Fill, then the tool window is floating.
        /// </remarks>
        /// <example>
        /// Here is a sample of how to use this method. It will add a form to the dock container with initial docking on Left.
        /// <code>
        /// private void OnCreateNewToolWindowDockedLeft (object sender, EventArgs e)
        /// {
        ///    // Create a new instance of the child form.
        ///    DockableToolWindow childForm = new DockableToolWindow ();
        /// 
        ///    // Add and dock the form in the left panel of the container
        ///    _dockContainer1.DockToolWindow (childForm, zDockMode.Left);
        /// 
        ///    // Show the form
        ///    childForm.Show ();
        /// }
        /// </code>
        /// </example>
        /// <param name="toolWindow">tool window to be docked. If the tool window is not in the container
        /// when this method is called, then the window is added as if 
        /// <see cref="AddToolWindow">AddToolWindow</see> method was invoked. </param>
        /// <param name="dockMode">dock mode of the tool window can be Left, Right, Top, Bottom, Fill or None, but not a combination
        /// of these.</param>
        public void DockToolWindow(ToolWindow toolWindow, DockMode dockMode)
        {
            if (IsInContainer(toolWindow) == false)
            {
                AddToolWindow(toolWindow);
            }

            _panels.DockToolWindow(toolWindow, dockMode);

            if (dockMode != DockMode.None)
            {
                _undockedToolWindows.Remove(toolWindow);

                SelectToolWindow(toolWindow);

                MoveInFront(_undockedToolWindows);
            }
        }

        /// <summary>
        /// Undock the tool window but doesn't remove it from conainer
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A <exception cref="NullReferenceException">NullReferenceException</exception> 
        /// will be thrown if the tool window is null.
        /// <br/>
        /// </remarks>
        /// <param name="toolWindow">tool window to be undocked</param>
        public void UndockToolWindow(ToolWindow toolWindow)
        {
            _panels.UndockToolWindow(toolWindow);

            toolWindow.TabButton.Reset();

            if (_undockedToolWindows.Contains(toolWindow) == false)
            {
                _undockedToolWindows.Add(toolWindow);
            }
        }

        /// <summary>
        /// Select the given tool window. 
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A <exception cref="NullReferenceException">NullReferenceException</exception> 
        /// will be thrown if the tool window is null.
        /// <br/>
        /// If the given tool window is floating, then is moved on z-axis in the top over all other tool windows.
        /// <br/>
        /// If the given tool window is docked, then is moved on z-axis in the top over all other tool windows docked
        /// in that panel. The floating tool windows remain top most.
        /// </remarks>
        /// <param name="toolWindow">tool window to be selected</param>
        public void SelectToolWindow(ToolWindow toolWindow)
        {
            if (toolWindow.DockMode == DockMode.None)
            {
                MoveInFront(toolWindow);
                toolWindow.Select();
                return;
            }

            ToolWindow[] panelToolWindows = _panels.GetPanelToolWindows(toolWindow.DockMode);

            foreach (ToolWindow panelToolWindow in panelToolWindows)
            {
                panelToolWindow.TabButton.Selected = false;
            }

            toolWindow.TabButton.Selected = true;

            MoveInFront(toolWindow);
            MoveInFront(_undockedToolWindows);
            toolWindow.Select();

            if (_panels.IsVisible(toolWindow) == false)
            {
                _panels.SetAutoHidden(toolWindow.DockMode, false);
            }

            Invalidate();
        }

        /// <summary>
        /// Detects if the panel identified by given dock mode is auto-hidden
        /// </summary>
        /// <param name="dockMode">dock mode can be Left, Rigth, Top, Bottom. 
        /// Other values will be ignored and the method will return false</param>
        /// <returns>true if the dock mode specifies a valid panel and that panel is auto-hidden</returns>
        public bool IsAutoHidden(DockMode dockMode)
        {
            SideDockPanel sidePanel = _panels.GetPanel(dockMode) as SideDockPanel;
            if (sidePanel == null)
            {
                return false;
            }

            return sidePanel.AutoHidden;
        }

        /// <summary>
        /// Detects if the panel identified by given dock mode is auto-hide
        /// </summary>
        /// <param name="dockMode">dock mode can be Left, Rigth, Top, Bottom. 
        /// Other values will be ignored and the method will return false</param>
        /// <returns>true if the dock mode specifies a valid panel and that panel is auto-hide</returns>
        public bool IsAutoHide(DockMode dockMode)
        {
            SideDockPanel sidePanel = _panels.GetPanel(dockMode) as SideDockPanel;
            if (sidePanel == null)
            {
                return false;
            }

            return sidePanel.AutoHide;
        }

        /// <summary>
        /// Change the auto-hide state of given panel
        /// </summary>
        /// <param name="dockMode">dock mode can be Left, Rigth, Top, Bottom. 
        /// Other values will be ignored and the method will return false</param>
        /// <param name="autoHideValue">new auto-hide value</param>
        /// <returns>true if the dock mode specifies a valid panel and auto-hide state was changed</returns>
        public bool SetAutoHide(DockMode dockMode, bool autoHideValue)
        {
            SideDockPanel sidePanel = _panels.GetPanel(dockMode) as SideDockPanel;
            if (sidePanel == null)
            {
                return false;
            }

            if (sidePanel.AutoHide == autoHideValue)
            {
                return false;
            }

            if (autoHideValue)
            {
                _panels.SetAutoHide(dockMode, true);
                _panels.SetAutoHidden(dockMode, true);
            }
            else
            {
                _panels.SetAutoHidden(dockMode, false);
                _panels.SetAutoHide(dockMode, false);
            }

            return true;
        }

        /// <summary>
        /// Get all the tool windows which are docked in the panel identified by the given dock mode.
        /// </summary>
        /// <remarks>
        /// If the dock mode identifies a valid panel, the result will be an array of tool windows. If no tool 
        /// window is docked on the panel, an empty array will be returned.
        /// <br/>
        /// If the dock mode doesn't identify a valid panel, then null is returned.
        /// </remarks>
        /// <param name="dockMode">dock mode which identifies the panel for which tool windows are requested.
        /// Valid values are Left, Right, Top, Bottom or Fill</param>
        /// <returns>Vector of tool windows from the identified panel.</returns>
        public ToolWindow[] GetDockedWindows(DockMode dockMode)
        {
            DockPanel panel = _panels.GetPanel(dockMode);
            if (panel == null)
            {
                return null;
            }

            return panel.ToolWindows;
        }

        /// <summary>
        /// Get the visible tool windows which are docked in the panel identified by the given dock mode.
        /// </summary>
        /// <remarks>
        /// If the dock mode identifies a valid panel, the result will be an array of tool windows. If no tool 
        /// window is docked on the panel, an empty array will be returned.
        /// <br/>
        /// If the dock mode doesn't identify a valid panel, then null is returned.
        /// </remarks>
        /// <param name="dockMode">dock mode which identifies the panel for which tool windows are requested.
        /// Valid values are Left, Right, Top, Bottom or Fill</param>
        /// <returns>Vector of tool windows from the identified panel.</returns>
        public ToolWindow[] GetVisibleDockedWindows(DockMode dockMode)
        {
            DockPanel panel = _panels.GetPanel(dockMode);
            if (panel == null)
            {
                return null;
            }

            return panel.VisibleToolWindows;
        }

        /// <summary>
        /// Gets the top tool window from the panel identified by given dock mode
        /// </summary>
        /// <param name="dockMode">dock mode to identify the panel from which the top tool window is requested</param>
        /// <returns>top tool window or null if no visible window is in the panel</returns>
        public ToolWindow GetTopToolWindow(DockMode dockMode)
        {
            return _panels.GetTopMostToolWindow(dockMode);
        }



        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                Paint -= OnPaint;
                MouseDown -= OnMouseDown;
                MouseUp -= OnMouseUp;
                MouseMove -= OnMouseMove;

                if (_mouseCheckTimer != null)
                {
                    _mouseCheckTimer.Enabled = false;
                    _mouseCheckTimer.Tick -= OnTestMouseState;
                    _mouseCheckTimer.Dispose();
                    _mouseCheckTimer = null;
                }

                while (_dockableToolWindows.Count > 0)
                {
                    RemoveToolWindow(_dockableToolWindows[0]);
                }

                if (_dockPreviewEngine != null)
                {
                    _dockPreviewEngine.VisibleChanged -= OnDockPreviewVisibleChanged;
                    _dockPreviewEngine = null;
                }

                if (_panels != null)
                {
                    _panels.MinimumSizeChanged -= OnMinimumSizeChanged;
                    _panels.Dispose();
                    _panels = null;
                }

                components.Dispose();
            }
            base.Dispose(disposing);
        }



        /// <summary>
        /// Handler for the mouse down event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            SelectToolWindowFromPoint(e.Location);
            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handler for the mouse up event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_undockCenterButtonHoover)
                {
                    if (CenterUndockButtonBounds.Contains(e.Location))
                    {
                        ToolWindow topmostToolWindow = _panels.GetTopMostToolWindow(DockMode.Fill);
                        UndockToolWindow(topmostToolWindow);
                    }
                }
                else if (_closeCenterButtonHoover)
                {
                    if (CenterCloseButtonBounds.Contains(e.Location))
                    {
                        ToolWindow topmostToolWindow = _panels.GetTopMostToolWindow(DockMode.Fill);
                        if (topmostToolWindow != null)
                        {
                            topmostToolWindow.Close();
                        }
                    }
                }
                else if (_menuCenterButtonHoover)
                {
                    if (CenterMenuButtonBounds.Contains(e.Location))
                    {
                        ToolWindow topmostToolWindow = _panels.GetTopMostToolWindow(DockMode.Fill);
                        if (topmostToolWindow != null)
                        {
                            RaiseContextMenuRequest(topmostToolWindow, e.Button);
                        }
                    }
                }
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handler for the mouse move event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = MousePosition;

            DetectMouseHoover(mousePosition, CenterCloseButtonBounds, ref _closeCenterButtonHoover);
            DetectMouseHoover(mousePosition, CenterMenuButtonBounds, ref _menuCenterButtonHoover);
            DetectMouseHoover(mousePosition, CenterUndockButtonBounds, ref _undockCenterButtonHoover);

            if (SelectToolWindowsOnHoover)
            {
                if (Math.Abs(_lastMouseLocation.X - mousePosition.X) > HooverMouseDelta ||
                    Math.Abs(_lastMouseLocation.Y - mousePosition.Y) > HooverMouseDelta)
                {
                    _lastMouseLocation = mousePosition;

                    Point location = PointToClient(mousePosition);
                    SelectToolWindowFromPoint(location);
                }
            }

            if (IsSplitterCursor() || _menuCenterButtonHoover || _closeCenterButtonHoover || _undockCenterButtonHoover)
            {
                IncreaseStateCheckFrequency();
            }
        }

        /// <summary>
        /// Handler for painting the control
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(SystemBrushes.AppWorkspace, ClientRectangle);

            // Draw splitters
            Rectangle splitterBounds;
            splitterBounds = _panels.GetPanelSplitterBounds(DockMode.Left);
            g.FillRectangle(SystemBrushes.Control, splitterBounds);
            g.DrawRectangle(SystemPens.ControlLight, splitterBounds);

            splitterBounds = _panels.GetPanelSplitterBounds(DockMode.Top);
            g.FillRectangle(SystemBrushes.Control, splitterBounds);
            g.DrawRectangle(SystemPens.ControlLight, splitterBounds);

            splitterBounds = _panels.GetPanelSplitterBounds(DockMode.Right);
            g.FillRectangle(SystemBrushes.Control, splitterBounds);
            g.DrawRectangle(SystemPens.ControlLight, splitterBounds);

            splitterBounds = _panels.GetPanelSplitterBounds(DockMode.Bottom);
            g.FillRectangle(SystemBrushes.Control, splitterBounds);
            g.DrawRectangle(SystemPens.ControlLight, splitterBounds);


            // Draw tab bouttons
            DrawHorizontalTabButtons(DockMode.Left, _panels.GetFixedButtonsBounds(DockMode.Left), _panels.GetPanelVisibleToolWindows(DockMode.Left), g);
            DrawHorizontalTabButtons(DockMode.Right, _panels.GetFixedButtonsBounds(DockMode.Right), _panels.GetPanelVisibleToolWindows(DockMode.Right), g);

            DrawVerticalTabButtons(DockMode.Left, _panels.GetPanelButtonsBounds(DockMode.Left), _panels.GetPanelVisibleToolWindows(DockMode.Left), g);
            DrawVerticalTabButtons(DockMode.Right, _panels.GetPanelButtonsBounds(DockMode.Right), _panels.GetPanelVisibleToolWindows(DockMode.Right), g);

            DrawHorizontalTabButtons(DockMode.Top, _panels.GetPanelButtonsBounds(DockMode.Top), _panels.GetPanelVisibleToolWindows(DockMode.Top), g);
            DrawHorizontalTabButtons(DockMode.Bottom, _panels.GetPanelButtonsBounds(DockMode.Bottom), _panels.GetPanelVisibleToolWindows(DockMode.Bottom), g);
            DrawHorizontalTabButtons(DockMode.Fill, _panels.GetPanelButtonsBounds(DockMode.Fill), _panels.GetPanelVisibleToolWindows(DockMode.Fill), g);
        }

        public void RedrawDrawTabButtons()
        {
            Invalidate(_panels.GetFixedButtonsBounds(DockMode.Left));
            Invalidate(_panels.GetFixedButtonsBounds(DockMode.Left));
            Invalidate(_panels.GetFixedButtonsBounds(DockMode.Right));
            Invalidate(_panels.GetPanelButtonsBounds(DockMode.Left));
            Invalidate(_panels.GetPanelButtonsBounds(DockMode.Right));
            Invalidate(_panels.GetPanelButtonsBounds(DockMode.Top));
            Invalidate(_panels.GetPanelButtonsBounds(DockMode.Bottom));
            Invalidate(_panels.GetPanelButtonsBounds(DockMode.Fill));
        }

        /// <summary>
        /// Timed handler used to check the mouse state
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnTestMouseState(object sender, EventArgs e)
        {
            Point mousePosition = MousePosition;
            MouseButtons button = MouseButtons;

            // Save processor
            Application.DoEvents();
            System.Threading.Thread.Sleep(1);

            if (button != MouseButtons.Left)
            {
                if (_panels.IsCursorChanged)
                {
                    _panels.UpdateMouseCursor(PointToClient(mousePosition));
                }

                _dockPreviewEngine.HideDockGuiders();

                UnlockMovedToolWindow();

                DetectMouseHoover(mousePosition, CenterCloseButtonBounds, ref _closeCenterButtonHoover);
                DetectMouseHoover(mousePosition, CenterMenuButtonBounds, ref _menuCenterButtonHoover);
                DetectMouseHoover(mousePosition, CenterUndockButtonBounds, ref _undockCenterButtonHoover);
            }

            UpdatePreviewEngine(mousePosition);

            if (button != MouseButtons.None)
            {
                return;
            }

            UpdateAutoHiddenState(DockMode.Left, mousePosition);
            UpdateAutoHiddenState(DockMode.Right, mousePosition);
            UpdateAutoHiddenState(DockMode.Top, mousePosition);
            UpdateAutoHiddenState(DockMode.Bottom, mousePosition);

            if (IsAutoHidden(DockMode.Left) == false && IsAutoHide(DockMode.Left))
            {
                return;
            }

            if (IsAutoHidden(DockMode.Right) == false && IsAutoHide(DockMode.Right))
            {
                return;
            }

            if (IsAutoHidden(DockMode.Top) == false && IsAutoHide(DockMode.Top))
            {
                return;
            }

            if (IsAutoHidden(DockMode.Bottom) == false && IsAutoHide(DockMode.Bottom))
            {
                return;
            }

            if (IsSplitterCursor())
            {
                return;
            }

            if (_closeCenterButtonHoover || _menuCenterButtonHoover || _undockCenterButtonHoover)
            {
                return;
            }

            DecreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the disposal of tool window
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnDisposeToolWindow(object sender, EventArgs e)
        {
            ToolWindow toolWindow = sender as ToolWindow;
            if (toolWindow != null)
            {
                RemoveToolWindow(toolWindow);
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the change of tool window parent
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowParentChanged(object sender, EventArgs e)
        {
            ToolWindow toolWindow = sender as ToolWindow;
            if (toolWindow != null)
            {
                if (toolWindow.Parent == this)
                {
                    return;
                }

                RemoveToolWindow(toolWindow);
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the move of tool window using the mouse
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowMove(object sender, EventArgs e)
        {
            Point mousePosition = MousePosition;
            MouseButtons button = MouseButtons;

            if (button != MouseButtons.Left)
            {
                return;
            }

            _movedToolWindow = sender as ToolWindow;
            if (_movedToolWindow == null)
            {
                return;
            }

            MoveInFront(_movedToolWindow);

            if (_dockPreviewEngine.Visibile)
            {
                return;
            }

            if (_movedToolWindow.TitleBarScreenBounds.Contains(mousePosition) == false)
            {
                _movedToolWindow = null;
                return;
            }

            if (_panels.IsVisible(_movedToolWindow) == false)
            {
                _movedToolWindow = null;
                return;
            }

            _dockPreviewEngine.ShowDockGuiders(_movedToolWindow.AllowedDock, RectangleToScreen(ClientRectangle));
            UndockToolWindow(_movedToolWindow);

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the visibility change in dock preview
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnDockPreviewVisibleChanged(object sender, EventArgs e)
        {
            if (_dockPreviewEngine.Visibile == false)
            {
                if (_movedToolWindow == null)
                {
                    return;
                }

                DockToolWindow(_movedToolWindow, _dockPreviewEngine.DockMode);
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the close of the tool window
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowClose(object sender, FormClosedEventArgs e)
        {
            ToolWindow toolWindow = sender as ToolWindow;
            if (toolWindow != null)
            {
                RemoveToolWindow(toolWindow);
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the dock change for tool windows. The dock is not allowed.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowDockChanged(object sender, EventArgs e)
        {
            ToolWindow toolWindow = sender as ToolWindow;
            if (toolWindow != null)
            {
                if (toolWindow.Dock != DockStyle.None)
                {
                    throw new NotSupportedException("DockableToolWindow can't be docked while in DockContainer");
                }
            }
        }

        /// <summary>
        /// Handle the auto-hide state change for tool windows.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowAutoHideChanged(object sender, EventArgs e)
        {
            IncreaseStateCheckFrequency();

            ToolWindow window = sender as ToolWindow;
            if (AutoHidePanelToggled != null && window != null)
            {
                AutoHideEventArgs args = new AutoHideEventArgs(window.DockMode);
                AutoHidePanelToggled(this, args);
            }
        }

        /// <summary>
        /// Handle the request for tool window context menu
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowContextMenuRequest(object sender, EventArgs e)
        {
            ToolWindow window = sender as ToolWindow;
            if (window != null && ContextMenuRequest != null)
            {
                ContextMenuEventArg args = new ContextMenuEventArg(window, MousePosition, MouseButtons);
                ContextMenuRequest(this, args);
            }
        }

        /// <summary>
        /// Handle the minimum container size changed
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMinimumSizeChanged(object sender, EventArgs e)
        {
            if (MinimumSizeChanged != null)
            {
                MinimumSizeChanged(this, EventArgs.Empty);
            }

            IncreaseStateCheckFrequency();
        }


        /// <summary>
        /// Raise context menu request
        /// </summary>
        /// <param name="selection">selection</param>
        /// <param name="buttons">mouse buttons</param>
        private void RaiseContextMenuRequest(ToolWindow selection, MouseButtons buttons)
        {
            EventHandler<ContextMenuEventArg> handler = ContextMenuRequest;
            if (handler != null)
            {
                ContextMenuEventArg args = new ContextMenuEventArg(selection, MousePosition, buttons);
                handler(this, args);
            }
        }

        /// <summary>
        /// Draw the horizontal tab buttons
        /// </summary>
        /// <param name="dockMode">dock mode used to identify the panel from which are the buttons</param>
        /// <param name="bounds">bounds of the area in which buttons can be drawn</param>
        /// <param name="toolWindows">collection of the tool windows in the panel</param>
        /// <param name="graphics">graphics context</param>
        private void DrawHorizontalTabButtons(DockMode dockMode, Rectangle bounds, ToolWindow[] toolWindows, Graphics graphics)
        {
            graphics.FillRectangle(SystemBrushes.Control, bounds);

            int windowsCount = toolWindows.Length;
            if (windowsCount == 0)
            {
                return;
            }

            if (dockMode == DockMode.Left || dockMode == DockMode.Right)
            {
                if (_panels.IsAutoHide(dockMode))
                {
                    return;
                }
            }

            RectangleF clip = graphics.ClipBounds;

            int width = bounds.Width;
            if (dockMode == DockMode.Fill)
            {
                width -= 70;  // give space for buttons: close, autohide and context menu
            }
            int maxButtonWidth = width / windowsCount;
            Rectangle buttonBounds = new Rectangle(bounds.Left, bounds.Top, maxButtonWidth, bounds.Height - 2);

            for (int index = 0; index < windowsCount; index++)
            {
                var button = toolWindows[index].TabButton;
                button.Draw(buttonBounds, Font, false, graphics);
                buttonBounds.X = button.Bounds.Right;
            }

            graphics.SetClip(clip);

            if (dockMode == DockMode.Fill)
            {
                DrawUtility.DrawUndockButton(CenterUndockButtonBounds, _undockCenterButtonHoover, graphics);
                DrawUtility.DrawContextMenuButton(CenterMenuButtonBounds, _menuCenterButtonHoover, graphics);
                DrawUtility.DrawCloseButton(CenterCloseButtonBounds, _closeCenterButtonHoover, graphics);
            }
        }

        /// <summary>
        /// Draw the vertical tab buttons
        /// </summary>
        /// <param name="dockMode">dock mode used to identify the panel from which are the buttons</param>
        /// <param name="bounds">bounds of the area in which buttons can be drawn</param>
        /// <param name="toolWindows">collection of the tool windows in the panel</param>
        /// <param name="graphics">graphics context</param>
        private void DrawVerticalTabButtons(DockMode dockMode, Rectangle bounds, ToolWindow[] toolWindows, Graphics graphics)
        {
            graphics.FillRectangle(SystemBrushes.Control, bounds);

            int windowsCount = toolWindows.Length;
            if (windowsCount == 0)
            {
                return;
            }

            if (dockMode == DockMode.Left || dockMode == DockMode.Right)
            {
                if (_panels.IsAutoHide(dockMode) == false)
                {
                    return;
                }
            }

            RectangleF clip = graphics.ClipBounds;

            int maxButtonHeight = bounds.Height / windowsCount;
            Rectangle buttonBounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width - 2, maxButtonHeight);

            for (int index = 0; index < windowsCount; index++)
            {
                TabButton button = (TabButton)toolWindows[index].TabButton;
                button.Draw(buttonBounds, Font, true, graphics);
                buttonBounds.Y = button.Bounds.Bottom;
            }

            graphics.SetClip(clip);
        }

        /// <summary>
        /// Update the auto-hidden state when the mouse position is outside of the panel
        /// </summary>
        /// <param name="dockMode">dock mode used to identify the panel</param>
        /// <param name="mousePosition">mouse position in screen coordinates</param>
        private void UpdateAutoHiddenState(DockMode dockMode, Point mousePosition)
        {
            if (_panels.IsAutoHide(dockMode) == false)
            {
                return;
            }

            Rectangle bounds = _panels.GetPanelNonHiddenBounds(dockMode);
            if (bounds.Contains(mousePosition))
            {
                return;
            }

            if (RectangleToScreen(_panels.GetPanelButtonsBounds(dockMode)).Contains(mousePosition))
            {
                return;
            }

            if (RectangleToScreen(_panels.GetPanelSplitterBounds(dockMode)).Contains(mousePosition))
            {
                return;
            }

            _panels.SetAutoHidden(dockMode, true);
        }

        /// <summary>
        /// Move the given tool window in the front of the view
        /// </summary>
        /// <param name="toolWindow">tool window to be moved in front of the view</param>
        private void MoveInFront(ToolWindow toolWindow)
        {
            Controls.SetChildIndex(toolWindow, 0);

            if (ToolWindowSelected != null)
            {
                ToolSelectionChangedEventArgs e = new ToolSelectionChangedEventArgs(toolWindow);
                ToolWindowSelected(this, e);
            }
        }

        /// <summary>
        /// Move the entire collection of tool windows in front of the view.
        /// The last tool window from the collection will became the top most window
        /// </summary>
        /// <param name="toolWindows">tool windows collection to be moved in front of the view</param>
        private void MoveInFront(List<ToolWindow> toolWindows)
        {
            foreach (ToolWindow toolWindow in toolWindows)
            {
                MoveInFront(toolWindow);
            }
        }

        /// <summary>
        /// Select tool window from point
        /// </summary>
        /// <param name="location">location</param>
        private void SelectToolWindowFromPoint(Point location)
        {
            foreach (TabButton button in _tabButtons)
            {
                if (button.Bounds.Contains(location))
                {
                    SelectToolWindow((ToolWindow)button.TitleData);
                    IncreaseStateCheckFrequency();
                    return;
                }
            }
        }

        /// <summary>
        /// Create a tab button associated with the given tool window
        /// </summary>
        /// <param name="toolWindow">tool window for which the tab button will be created</param>
        private void CreateTabButton(ToolWindow toolWindow)
        {
            TabButton button = new TabButton(this, toolWindow);
            toolWindow.TabButton = button;

            button.NotSelectedColor = TabButtonNotSelectedColor;
            button.SelectedBackColor1 = TabButtonSelectedBackColor1;
            button.SelectedBackColor2 = TabButtonSelectedBackColor2;
            button.SelectedBorderColor = TabButtonSelectedBorderColor;
            button.SelectedColor = TabButtonSelectedColor;
            button.ShowSelection = TabButtonShowSelection;

            _tabButtons.Add(button);
        }

        /// <summary>
        /// Remove the tab button associated with the given tool window
        /// </summary>
        /// <param name="toolWindow">tool window for which the tab button must be removed</param>
        private void RemoveTabButton(ToolWindow toolWindow)
        {
            _tabButtons.Remove((TabButton)toolWindow.TabButton);

            toolWindow.TabButton = null;
        }

        /// <summary>
        /// Unlocks the moved tool window
        /// </summary>
        private void UnlockMovedToolWindow()
        {
            if (_movedToolWindow != null)
            {
                if (_movedToolWindow.IsDocked == false)
                {
                    _movedToolWindow.UnlockFormSize();
                }
                _movedToolWindow = null;
            }
        }

        /// <summary>
        /// Detect mouse hoover over given bounds
        /// </summary>
        /// <param name="mousePosition">mouse position in screen coordinates</param>
        /// <param name="hooverBounds">hoover bounds in client coordinates</param>
        /// <param name="hoover">hoover flag indicating if mouse was hoovering over the given bounds</param>
        private void DetectMouseHoover(Point mousePosition, Rectangle hooverBounds, ref bool hoover)
        {
            if (hooverBounds.Contains(PointToClient(mousePosition)))
            {
                if (hoover == false)
                {
                    hoover = true;
                    Invalidate();
                }
            }
            else if (hoover)
            {
                hoover = false;
                Invalidate();
            }
        }

        /// <summary>
        /// Update the preview engine
        /// </summary>
        /// <param name="mousePosition">mouse position in screen coordinates</param>
        private void UpdatePreviewEngine(Point mousePosition)
        {
            if (_dockPreviewEngine.Visibile == false)
            {
                return;
            }

            _dockPreviewEngine.LeftPreviewBounds = _panels.GetPanelNonHiddenBounds(DockMode.Left);
            _dockPreviewEngine.RightPreviewBounds = _panels.GetPanelNonHiddenBounds(DockMode.Right);
            _dockPreviewEngine.TopPreviewBounds = _panels.GetPanelNonHiddenBounds(DockMode.Top);
            _dockPreviewEngine.BottomPreviewBounds = _panels.GetPanelNonHiddenBounds(DockMode.Bottom);
            _dockPreviewEngine.FillPreviewBounds = _panels.GetPanelNonHiddenBounds(DockMode.Fill);

            _dockPreviewEngine.UpdateDockPreviewOnMouseMove(mousePosition);
        }

        /// <summary>
        /// Bounds of the close button from the center panel
        /// </summary>
        private Rectangle CenterCloseButtonBounds
        {
            get
            {
                Rectangle bounds = _panels.GetPanelButtonsBounds(DockMode.Fill);
                return new Rectangle(bounds.Right - 18, bounds.Y + (bounds.Height - 12) / 2, 12, 12);
            }
        }

        /// <summary>
        /// Bounds of the menu button from the center panel
        /// </summary>
        private Rectangle CenterMenuButtonBounds
        {
            get
            {
                Rectangle bounds = _panels.GetPanelButtonsBounds(DockMode.Fill);
                return new Rectangle(bounds.Right - 38, bounds.Y + (bounds.Height - 12) / 2, 12, 12);
            }
        }

        private Rectangle CenterUndockButtonBounds
        {
            get
            {
                Rectangle bounds = _panels.GetPanelButtonsBounds(DockMode.Fill);
                return new Rectangle(bounds.Right - 58, bounds.Y + (bounds.Height - 12) / 2, 12, 12);
            }
        }

        /// <summary>
        /// Increase state check frequency
        /// </summary>
        private void IncreaseStateCheckFrequency()
        {
            if (_mouseCheckTimer != null)
            {
                _mouseCheckTimer.Interval = 100;
            }
        }

        /// <summary>
        /// Decrease state check frequency
        /// </summary>
        private void DecreaseStateCheckFrequency()
        {
            if (_mouseCheckTimer != null)
            {
                _mouseCheckTimer.Interval = 10000;
            }
        }

        /// <summary>
        /// Checks if the cursor is splitter
        /// </summary>
        /// <returns>true if the cursor is splitter</returns>
        private bool IsSplitterCursor()
        {
            return Cursor == Cursors.HSplit || Cursor == Cursors.VSplit;
        }

    }
}
