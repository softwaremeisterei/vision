using System;
using System.Drawing;
using System.Windows.Forms;

namespace Docking.Controls
{
    /// <summary>
    /// Implements resize of the DockPanels using the splitter.
    /// </summary>
    internal class DockPanelsResizer : IDisposable
    {
        #region Fields.

        private const int SplitterDimension = 3;

        private Control _container = null;
        private Form _splitter = new Form();
        private DockMode _resizedPanel = DockMode.None;
        private int _minViewWidth = DockPanelsLayout.MinViewHeight;
        private int _minViewHeight = DockPanelsLayout.MinViewHeight;
        private bool _isCursorChanged = false;

        private DockPanelsLayout _layout = null;

        #endregion Fields.

        #region Instance.

        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="container">container</param>
        public DockPanelsResizer(Control container)
        {
            _layout = new DockPanelsLayout();
            _layout.UpdateLayoutRequested += new EventHandler(OnUpdateLayoutRequested);

            _container = container;

            _container.Resize += OnContainerResize;
            _container.MouseDown += OnMouseDownInContainer;
            _container.MouseMove += OnMouseMovedInContainer;
            _container.MouseUp += OnMouseUpInContainer;

            _splitter.FormBorderStyle = FormBorderStyle.None;
            _splitter.MinimumSize = new Size(1, 1);
            _splitter.Width = 1;
            _splitter.ShowInTaskbar = false;
            _splitter.TopMost = true;
            _splitter.BackColor = Color.DarkGray;
            _splitter.Opacity = 0.90;
            _splitter.StartPosition = FormStartPosition.Manual;
            _splitter.AutoScaleMode = AutoScaleMode.None;
            _splitter.Margin = new Padding();
            _splitter.AutoScaleDimensions = new SizeF(1, 1);
            _splitter.Bounds = new Rectangle(0, 0, 2, 2);

            UpdatePanelsLayout();
        }

        #endregion Instance.

        #region IDisposable.

        /// <summary>
        /// Distrugere
        /// </summary>
        public void Dispose()
        {
            if (_container != null)
            {
                _container.Resize -= OnContainerResize;
                _container.MouseDown -= OnMouseDownInContainer;
                _container.MouseMove -= OnMouseMovedInContainer;
                _container.MouseUp -= OnMouseUpInContainer;
            }

            if (_splitter != null)
            {
                _splitter.Dispose();
                _splitter = null;
            }

            if (_container != null)
            {
                _container = null;
            }

            if (_layout != null)
            {
                _layout.Dispose();
                _layout = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable.

        #region Public section.

        /// <summary>
        /// This event occurs when the minimum allowed size for the container was changed.
        /// The form on which this container is placed should display the entire container
        /// </summary>
        public event EventHandler MinimumSizeChanged;

        /// <summary>
        /// Get the panel with the given dock mode
        /// </summary>
        /// <param name="dockMode">dock mode which can be Left, Right, Top, Bottom or Fill</param>
        /// <returns>panel with given dock mode</returns>
        public DockPanel GetPanel(DockMode dockMode)
        {
            switch (dockMode)
            {
                case DockMode.Left:
                    return _layout.LeftPanel;

                case DockMode.Right:
                    return _layout.RightPanel;

                case DockMode.Top:
                    return _layout.TopPanel;

                case DockMode.Bottom:
                    return _layout.BottomPanel;

                case DockMode.Fill:
                    return _layout.CenterPanel;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Dock the tool window to the specified panel.
        /// </summary>
        /// <param name="toolWindow">tool window to be docked</param>
        /// <param name="dockMode">dock mode can be Left, Right, Top, Bottom and Fill</param>
        public void DockToolWindow(ToolWindow toolWindow, DockMode dockMode)
        {
            DockPanel panel = GetPanel(dockMode);

            if (panel != null)
            {
                panel.DockToolWindow(toolWindow);
            }
        }

        /// <summary>
        /// Undock the specified tool window
        /// </summary>
        /// <param name="toolWindow">tool window to be undocked</param>
        public void UndockToolWindow(ToolWindow toolWindow)
        {
            DockPanel panel = GetPanel(toolWindow.DockMode);
            if (panel != null)
            {
                panel.UndockToolWindow(toolWindow);
            }
        }

        /// <summary>
        /// Checks if the given tool window is docked in a side panel which is hidden.
        /// </summary>
        /// <param name="toolWindow">tool window to be checked</param>
        /// <returns>setat daca toolWindow nu e in nici un panou sau daca e intr-un panou vizibil</returns>
        public bool IsVisible(ToolWindow toolWindow)
        {
            SideDockPanel panel = GetPanel(toolWindow.DockMode) as SideDockPanel;
            if (panel != null)
            {
                return panel.AutoHidden == false;
            }

            return toolWindow.Visible;
        }

        /// <summary>
        /// Checks if the panel with given dock mode has the AutoHide flag set.
        /// </summary>
        /// <param name="dockMode">dock mode to identify the panel to check. 
        /// Allowed values are Left, Right, Top, Bottom. Other values will be ignored and false will be returned.</param>
        /// <returns>the value of auto-hide flag for identified panel or false if no valid panel was specified.</returns>
        public bool IsAutoHide(DockMode dockMode)
        {
            SideDockPanel sideDock = GetPanel(dockMode) as SideDockPanel;
            if (sideDock == null)
            {
                return false;
            }

            return sideDock.AutoHide;
        }

        /// <summary>
        /// Set auto-hide flag for the panel with given dock mode
        /// </summary>
        /// <param name="dockMode">dock mode to identify the panel to change. 
        /// Allowed values are Left, Right, Top, Bottom. Other values will be ignored.</param>
        /// <param name="autoHidden">new auto-hide value</param>
        public void SetAutoHide(DockMode dockMode, bool autoHide)
        {
            SideDockPanel sideDock = GetPanel(dockMode) as SideDockPanel;
            if (sideDock == null)
            {
                return;
            }

            sideDock.AutoHide = autoHide;
        }

        /// <summary>
        /// Set auto-hidden flag for the panel with given dock mode
        /// </summary>
        /// <param name="dockMode">dock mode to identify the panel to change. 
        /// Allowed values are Left, Right, Top, Bottom. Other values will be ignored.</param>
        /// <param name="autoHidden">new auto-hidden value</param>
        public void SetAutoHidden(DockMode dockMode, bool autoHidden)
        {
            SideDockPanel sidePanel = GetPanel(dockMode) as SideDockPanel;
            if (sidePanel == null)
            {
                return;
            }

            if (sidePanel.AutoHide == false)
            {
                return;
            }

            sidePanel.AutoHidden = autoHidden;

            UpdatePanelsLayout();
        }

        /// <summary>
        /// Get the fixed buttons bounds in screen coordinates
        /// </summary>
        /// <remarks>
        /// throws a NotSupportedException if the panel is not found using the dockMode criteria
        /// </remarks>
        /// <param name="dockMode">dock mode of the panel for which bounds are requested.
        /// Valid values are Left and Right.</param>
        /// <returns>bounds of the region in which buttons can be drawn for the panel identified by dockMode</returns>
        public Rectangle GetFixedButtonsBounds(DockMode panou)
        {
            if (panou == DockMode.Left)
            {
                return _layout.LeftBottomButtonsBounds;
            }

            if (panou == DockMode.Right)
            {
                return _layout.RightBottomButtonsBounds;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Get the panel buttons bounds in screen coordinates
        /// </summary>
        /// <remarks>
        /// throws a NotSupportedException if the panel is not found using the dockMode criteria
        /// </remarks>
        /// <param name="dockMode">dock mode of the panel for which bounds are requested.
        /// Valid values are Left, Right, Top, Bottom and Fill.</param>
        /// <returns>bounds of the region in which buttons can be drawn for the panel identified by dockMode</returns>
        public Rectangle GetPanelButtonsBounds(DockMode dockMode)
        {
            DockPanel panel = GetPanel(dockMode);
            if (panel == null)
            {
                throw new NotSupportedException();
            }

            return panel.ButtonsBounds;
        }

        /// <summary>
        /// Get the panel bounds in screen coordinates, when the panel is not hidden
        /// </summary>
        /// <param name="dockMode">dock mode of the panel for which bounds are requested</param>
        /// <returns>panel bounds in screen coordinates (computed when the panel is not hidden)</returns>
        public Rectangle GetPanelNonHiddenBounds(DockMode dockMode)
        {
            if (dockMode == DockMode.Left)
            {
                return _container.RectangleToScreen(_layout.LeftPanel.PreviewBounds);
            }

            if (dockMode == DockMode.Right)
            {
                return _container.RectangleToScreen(_layout.RightPanel.PreviewBounds);
            }

            if (dockMode == DockMode.Top)
            {
                return _container.RectangleToScreen(_layout.TopPanel.PreviewBounds);
            }

            if (dockMode == DockMode.Bottom)
            {
                return _container.RectangleToScreen(_layout.BottomPanel.PreviewBounds);
            }

            if (dockMode == DockMode.Fill)
            {
                return _container.RectangleToScreen(_layout.CenterPanel.PreviewBounds);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Get the bounds (in screen coordinates) of the splitter attached to 
        /// the panel identified by the dockMode
        /// </summary>
        /// <remarks>
        /// throws a NotSupportedException if the panel is not found using the dockMode criteria
        /// </remarks>
        /// <param name="dockMode">dock mode of the panel for which bounds are requested.
        /// Valid values are Left, Right, Top, Bottom.</param>
        /// <returns>bounds of the splitter</returns>
        public Rectangle GetPanelSplitterBounds(DockMode dockMode)
        {
            SideDockPanel panel = GetPanel(dockMode) as SideDockPanel;
            if (panel == null)
            {
                throw new NotSupportedException();
            }

            return panel.SplitterBounds;
        }

        /// <summary>
        /// Get all the tool windows docked on the panel identified by given dock mode parameter
        /// </summary>
        /// <remarks>
        /// throws a NotSupportedException if the panel is not found using the dockMode criteria
        /// </remarks>
        /// <param name="dockMode">dock mode of the panel for which bounds are requested.
        /// Valid values are Left, Right, Top, Bottom and Fill.</param>
        /// <returns>vector of tool windows from the panel</returns>
        public ToolWindow[] GetPanelToolWindows(DockMode dockMode)
        {
            DockPanel panel = GetPanel(dockMode);
            if (panel == null)
            {
                throw new NotSupportedException();
            }

            return panel.ToolWindows;
        }

        /// <summary>
        /// Get all the visible tool windows docked on the panel identified by given dock mode parameter
        /// </summary>
        /// <remarks>
        /// throws a NotSupportedException if the panel is not found using the dockMode criteria
        /// </remarks>
        /// <param name="dockMode">dock mode of the panel for which bounds are requested.
        /// Valid values are Left, Right, Top, Bottom and Fill.</param>
        /// <returns>vector of tool windows from the panel</returns>
        public ToolWindow[] GetPanelVisibleToolWindows(DockMode dockMode)
        {
            DockPanel panel = GetPanel(dockMode);
            if (panel == null)
            {
                throw new NotSupportedException();
            }

            return panel.VisibleToolWindows;
        }

        /// <summary>
        /// Update the mouse cursor depending on the mouse position
        /// </summary>
        /// <param name="mousePosition">mouse position in screen coordinates</param>
        public void UpdateMouseCursor(Point mousePosition)
        {
            if (_layout.LeftPanel.SplitterBounds.Contains(mousePosition))
            {
                _container.Cursor = Cursors.VSplit;
                _isCursorChanged = true;
            }
            else if (_layout.RightPanel.SplitterBounds.Contains(mousePosition))
            {
                _container.Cursor = Cursors.VSplit;
                _isCursorChanged = true;
            }
            else if (_layout.TopPanel.SplitterBounds.Contains(mousePosition))
            {
                _container.Cursor = Cursors.HSplit;
                _isCursorChanged = true;
            }
            else if (_layout.BottomPanel.SplitterBounds.Contains(mousePosition))
            {
                _container.Cursor = Cursors.HSplit;
                _isCursorChanged = true;
            }
            else
            {
                _container.Cursor = Cursors.Default;
                _isCursorChanged = false;
            }
        }

        /// <summary>
        /// Flag indicating if the cursor is changed due to <see cref="UpdateMouseCursor">UpdateMouseCursor</see> call.
        /// </summary>
        public bool IsCursorChanged
        {
            get { return _isCursorChanged; }
        }

        /// <summary>
        /// Gets the top most tool window from the panel identified by given dock mode
        /// </summary>
        /// <param name="dockMode">dock mode of the panel for which bounds are requested.
        /// Valid values are Left, Right, Top, Bottom and Fill.</param>
        /// <returns>the tool window which is in the top of z-order on the panel identified by dock mode</returns>
        public ToolWindow GetTopMostToolWindow(DockMode dockMode)
        {
            ToolWindow topMost = null; // Is the window with smallest index in the container collection

            int smallestIndex = Int32.MaxValue;

            ToolWindow[] toolWindows = GetPanelVisibleToolWindows(dockMode);
            foreach (ToolWindow toolWindow in toolWindows)
            {
                int zOrderIndex = _container.Controls.GetChildIndex(toolWindow);
                if (zOrderIndex < smallestIndex)
                {
                    topMost = toolWindow;
                    smallestIndex = zOrderIndex;
                }
            }

            return topMost;
        }

        /// <summary>
        /// Left panel width
        /// </summary>
        public int LeftPanelWidth
        {
            get { return _layout.LeftPanel.NotHiddenDimension; }
            set
            {
                int newRight = value + _layout.LeftPanel.ContentBounds.Left;
                newRight = Math.Min(_layout.LeftPanel.MaxSlidePos, Math.Max(_layout.LeftPanel.MinSlidePos, newRight));
                _layout.LeftPanel.NotHiddenDimension = Math.Max(DockPanelsLayout.MinPanelDimension, newRight - _layout.LeftPanel.ContentBounds.Left);
            }
        }

        /// <summary>
        /// Right panel width
        /// </summary>
        public int RightPanelWidth
        {
            get { return _layout.RightPanel.NotHiddenDimension; }
            set
            {
                int newLeft = _layout.RightPanel.ContentBounds.Right - value;
                newLeft = Math.Min(_layout.RightPanel.MaxSlidePos, Math.Max(_layout.RightPanel.MinSlidePos, newLeft));
                _layout.RightPanel.NotHiddenDimension = Math.Max(DockPanelsLayout.MinPanelDimension, _layout.RightPanel.ContentBounds.Right - newLeft);
            }
        }

        /// <summary>
        /// Top panel height
        /// </summary>
        public int TopPanelHeight
        {
            get { return _layout.TopPanel.NotHiddenDimension; }
            set
            {
                int newBottom = value + _layout.TopPanel.ContentBounds.Top;
                newBottom = Math.Min(_layout.TopPanel.MaxSlidePos, Math.Max(_layout.TopPanel.MinSlidePos, newBottom));
                _layout.TopPanel.NotHiddenDimension = Math.Max(DockPanelsLayout.MinPanelDimension, newBottom - _layout.TopPanel.ContentBounds.Top);
            }
        }

        /// <summary>
        /// Bottom panel height
        /// </summary>
        public int BottomPanelHeight
        {
            get { return _layout.BottomPanel.NotHiddenDimension; }
            set
            {
                int newTop = _layout.BottomPanel.ContentBounds.Bottom - value;
                newTop = Math.Min(_layout.BottomPanel.MaxSlidePos, Math.Max(_layout.BottomPanel.MinSlidePos, newTop));
                _layout.BottomPanel.NotHiddenDimension = Math.Max(DockPanelsLayout.MinPanelDimension, _layout.BottomPanel.ContentBounds.Bottom - newTop);
            }
        }

        #endregion Public section.

        #region Private section.
        #region Received events.

        /// <summary>
        /// La apasare button cursor in container
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseDownInContainer(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            Rectangle splitterBounds = new Rectangle();
            Cursor cursor = null;
            _resizedPanel = DockMode.None;

            if (_layout.RightPanel.SplitterBounds.Contains(e.Location))
            {
                cursor = Cursors.VSplit;
                splitterBounds = _container.RectangleToScreen(_layout.RightPanel.SplitterBounds);
                _resizedPanel = DockMode.Right;
            }
            else if (_layout.LeftPanel.SplitterBounds.Contains(e.Location))
            {
                cursor = Cursors.VSplit;
                splitterBounds = _container.RectangleToScreen(_layout.LeftPanel.SplitterBounds);
                _resizedPanel = DockMode.Left;
            }
            else if (_layout.TopPanel.SplitterBounds.Contains(e.Location))
            {
                cursor = Cursors.HSplit;
                splitterBounds = _container.RectangleToScreen(_layout.TopPanel.SplitterBounds);
                _resizedPanel = DockMode.Top;
            }
            else if (_layout.BottomPanel.SplitterBounds.Contains(e.Location))
            {
                cursor = Cursors.HSplit;
                splitterBounds = _container.RectangleToScreen(_layout.BottomPanel.SplitterBounds);
                _resizedPanel = DockMode.Bottom;
            }

            if (splitterBounds.Width != 0 && splitterBounds.Height != 0 && cursor != null)
            {
                // First reset splitter to prevent flickering - this is not normal but is happening in ms windows
                _splitter.Bounds = new Rectangle(-100, -100, 1, 1);
                _splitter.Visible = true;

                // Then set the splitter bounds
                _splitter.Bounds = splitterBounds;
                _splitter.Cursor = cursor;
                _splitter.Visible = true;
                _container.Cursor = cursor;
            }
        }

        /// <summary>
        /// La mutare cursor in container
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseMovedInContainer(object sender, MouseEventArgs e)
        {
            Point cursorPos = Control.MousePosition;

            if (_splitter.Visible && _resizedPanel != DockMode.None)
            {
                _container.Cursor = _splitter.Cursor;

                if (_resizedPanel == DockMode.Left)
                {
                    _splitter.Left = Math.Min(Math.Max(ScreenX(_layout.LeftPanel.MinSlidePos), cursorPos.X), ScreenX(_layout.LeftPanel.MaxSlidePos));
                }
                else if (_resizedPanel == DockMode.Right)
                {
                    _splitter.Left = Math.Min(Math.Max(ScreenX(_layout.RightPanel.MinSlidePos), cursorPos.X), ScreenX(_layout.RightPanel.MaxSlidePos));
                }
                else if (_resizedPanel == DockMode.Top)
                {
                    _splitter.Top = Math.Min(Math.Max(ScreenY(_layout.TopPanel.MinSlidePos), cursorPos.Y), ScreenY(_layout.TopPanel.MaxSlidePos));
                }
                else if (_resizedPanel == DockMode.Bottom)
                {
                    _splitter.Top = Math.Min(Math.Max(ScreenY(_layout.BottomPanel.MinSlidePos), cursorPos.Y), ScreenY(_layout.BottomPanel.MaxSlidePos));
                }

                _isCursorChanged = true;
            }
            else
            {
                UpdateMouseCursor(e.Location);
            }
        }

        /// <summary>
        /// La ridicare button cursor din container
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseUpInContainer(object sender, MouseEventArgs e)
        {
            Point cursorPos = Control.MousePosition;

            if (_resizedPanel == DockMode.Left)
            {
                int screenRight = Math.Min(Math.Max(ScreenX(_layout.LeftPanel.MinSlidePos), cursorPos.X), ScreenX(_layout.LeftPanel.MaxSlidePos));
                LeftPanelWidth = ClientX(screenRight) - _layout.LeftPanel.ContentBounds.X;
            }
            else if (_resizedPanel == DockMode.Right)
            {
                int screenLeft = Math.Min(Math.Max(ScreenX(_layout.RightPanel.MinSlidePos), cursorPos.X), ScreenX(_layout.RightPanel.MaxSlidePos));
                RightPanelWidth = _layout.RightPanel.ContentBounds.Right - ClientX(screenLeft);
            }
            else if (_resizedPanel == DockMode.Top)
            {
                int screenBottom = Math.Min(Math.Max(ScreenY(_layout.TopPanel.MinSlidePos), cursorPos.Y), ScreenY(_layout.TopPanel.MaxSlidePos));
                TopPanelHeight = ClientY(screenBottom) - _layout.TopPanel.ContentBounds.Top;
            }
            else if (_resizedPanel == DockMode.Bottom)
            {
                int screenTop = Math.Min(Math.Max(ScreenY(_layout.BottomPanel.MinSlidePos), cursorPos.Y), ScreenY(_layout.BottomPanel.MaxSlidePos));
                BottomPanelHeight = _layout.BottomPanel.ContentBounds.Bottom - ClientY(screenTop);
            }

            _splitter.Visible = false;
            _container.Cursor = Cursors.Default;

            if (_resizedPanel != DockMode.None)
            {
                _resizedPanel = DockMode.None;
                UpdatePanelsLayout();
            }
        }

        /// <summary>
        /// La redimensionare container
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnContainerResize(object sender, EventArgs e)
        {
            UpdatePanelsLayout();
        }

        /// <summary>
        /// On request to update the layout
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnUpdateLayoutRequested(object sender, EventArgs e)
        {
            UpdatePanelsLayout();
        }

        #endregion Received events.

        /// <summary>
        /// Actualizeaza dimensiunile panourilor
        /// </summary>
        /// <returns>modurile actualizate</returns>
        public DockMode UpdatePanelsLayout()
        {
            DockMode modActualizat = DockMode.None;
            if (_layout.UpdateLeftPanelLayout(_container.ClientSize))
            {
                modActualizat |= DockMode.Left;
            }

            if (_layout.UpdateRightPanelLayout(_container.ClientSize))
            {
                modActualizat |= DockMode.Right;
            }

            if (_layout.UpdateTopPanelLayout(_container.ClientSize))
            {
                modActualizat |= DockMode.Top;
            }

            if (_layout.UpdateBottomPanelLayout(_container.ClientSize))
            {
                modActualizat |= DockMode.Bottom;
            }

            if (_layout.UpdateCenterPanelLayout(_container.ClientSize))
            {
                modActualizat |= DockMode.Fill;
            }


            int minimumWidth =
               // Fit left panel
               DockPanelsLayout.ButtonsPanelDimension + _layout.LeftPanel.NotHiddenDimension + SplitterDimension +
               // Fit right panel
               DockPanelsLayout.ButtonsPanelDimension + _layout.RightPanel.NotHiddenDimension + SplitterDimension +
               // Fit center panel
               DockPanelsLayout.MinPanelDimension;

            int minimumHeight =
               // Fit top panel
               DockPanelsLayout.ButtonsPanelDimension + _layout.TopPanel.NotHiddenDimension + SplitterDimension +
               // Fit bottom panel
               DockPanelsLayout.ButtonsPanelDimension + _layout.BottomPanel.NotHiddenDimension + SplitterDimension +
               // Fit center panel
               DockPanelsLayout.ButtonsPanelDimension + DockPanelsLayout.MinPanelDimension;

            minimumWidth = Math.Min(_container.Width, minimumWidth);
            minimumHeight = Math.Min(_container.Height, minimumHeight);

            if (minimumWidth != _minViewWidth || minimumHeight != _minViewHeight)
            {
                _minViewWidth = minimumWidth;
                _minViewHeight = minimumHeight;
                _container.MinimumSize = new Size(_minViewWidth, _minViewHeight);

                if (MinimumSizeChanged != null)
                {
                    MinimumSizeChanged(this, EventArgs.Empty);
                }
            }

            _container.Invalidate();

            return modActualizat;
        }

        /// <summary>
        /// Converts x axis coordinate from screen to container client area.
        /// </summary>
        /// <param name="screenX">screen x coordinate</param>
        /// <returns>container client x coordinate</returns>
        private int ClientX(int screenX)
        {
            Point point = new Point(screenX, 0);
            point = _container.PointToClient(point);
            return point.X;
        }

        /// <summary>
        /// Converts y axis coordinate from screen to container client area.
        /// </summary>
        /// <param name="screenY">screen y coordinate</param>
        /// <returns>container client y coordinate</returns>
        private int ClientY(int screenY)
        {
            Point point = new Point(0, screenY);
            point = _container.PointToClient(point);
            return point.Y;
        }

        /// <summary>
        /// Converts x axis coordinate from container client area to screen.
        /// </summary>
        /// <param name="clientX">container client x coordinate</param>
        /// <returns>screen x coordinate</returns>
        private int ScreenX(int clientX)
        {
            Point point = new Point(clientX, 0);
            point = _container.PointToScreen(point);
            return point.X;
        }

        /// <summary>
        /// Converts y axis coordinate from container client area to screen.
        /// </summary>
        /// <param name="clientY">container client y coordinate</param>
        /// <returns>screen y coordinate</returns>
        private int ScreenY(int clientY)
        {
            Point point = new Point(0, clientY);
            point = _container.PointToScreen(point);
            return point.Y;
        }

        #endregion Private section.
    }
}
