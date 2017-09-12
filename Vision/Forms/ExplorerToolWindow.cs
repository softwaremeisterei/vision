using Docking.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.BL;
using Vision.BL.Model;
using Vision.Lib;

namespace Vision.Forms
{
    /// <summary>
    /// Dockable tool window implements basic functionalities required to allow tool windows to be docked using
    /// <see cref="DockContainer">DockContainer</see>
    /// </summary>
    /// <remarks>Use this object as base class for your auto-dockable tool windows.</remarks>
    public class ExplorerToolWindow : ToolWindow, ISearchable
    {
        Timer _timer = new Timer();
        Context _context;
        Persistor _persistor;
        private bool _dirty = false;
        private static bool _modified = false;
        private string _currentProjectFilename;
        FindForm _findForm;
        private TreeView treeView1;
        private Button expandButton;
        private Button collapseButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem openProjectToolStripMenuItem;
        private ToolStripMenuItem newProjectToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem findToolStripMenuItem;
        private ToolStripMenuItem findNextToolStripMenuItem;
        private ToolStripMenuItem findPrevToolStripMenuItem;
        private ToolStripMenuItem contentToolStripMenuItem;
        private ToolStripMenuItem addToplevelNodeToolStripMenuItem;
        private ToolStripMenuItem addChildNodeToolStripMenuItem;
        private ToolStripMenuItem addSiblingNodeToolStripMenuItem;
        private ToolStripMenuItem moveNodeDownToolStripMenuItem;
        private ToolStripMenuItem modeNodeUpToolStripMenuItem;
        private ContextMenuStrip _docMenu;

        public ExplorerToolWindow()
        {
            this.InitializeComponent();

            _context = new Context();
            _persistor = new Persistor();
            _findForm = new Forms.FindForm(this);
            InitContextMenu();
            LoadLastProject();

            ActiveControl = treeView1;
            ClearDirtyFlag();
            SetupTimer();
        }

        private void SetupTimer()
        {
            _timer.Tick += new EventHandler(TimerEventProcessor);
            _timer.Interval = 2000;
            _timer.Start();
        }

        private void TimerEventProcessor(Object myObject,
                                            EventArgs myEventArgs)
        {
            if (_modified)
            {
                string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var backupFilepath = Path.Combine(rootPath, "Vision.backup.txt");
                Export(backupFilepath);
                _modified = false;
            }
        }

        private void DockableExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_dirty)
            {
                var dialogResult = MessageBox.Show("You have unsaved changes.\nDo you want to save before closing?", "Warning", MessageBoxButtons.YesNoCancel);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        if (!SaveProject())
                        {
                            e.Cancel = true;
                        };
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void saveFileMenuItem_Click(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void openFileMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Vision projects (*.visionproj)|*.visionproj";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                LoadProject(fileName);
            }
        }

        private void exportFileMenuItem_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void addToplevelNodeMenuItem_Click(object sender, EventArgs e)
        {
            var treeNode = AddNode();

            if (treeNode != null)
            {
                treeNode.BeginEdit();
            }
        }

        private void addChildNodeMenuItem_Click(object sender, EventArgs e)
        {
            AddChildNode();
        }

        private void addSiblingNodeMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count == 0)
            {
                var treeNode = AddNode();

                if (treeNode != null)
                {
                    treeNode.BeginEdit();
                }
            }
            else if (treeView1.SelectedNode != null)
            {
                var parentNode = (Node)treeView1.SelectedNode.Parent?.Tag;
                var treeNode = AddNode(parentNode);

                if (treeNode != null)
                {
                    treeNode.BeginEdit();
                }
            }
        }

        private void DisplayContent(Node node)
        {
            switch (node.DisplayType)
            {
                case DisplayType.Folder:
                    break;
                case DisplayType.Browser:
                    var browserForm = MainForm.GetInstance().OpenBrowserForm(node.Title, node.Url);
                    browserForm.SetTag(node);
                    browserForm.Navigate(node.Url);
                    browserForm.SetDocumentCompletedHandler(_contentWebBrowser_DocumentCompleted);
                    break;
            }
        }

        private void _contentWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var webBrowser = (WebBrowser)sender;

            if (webBrowser.Url == null)
            {
                return;
            }

            var node = (Node)webBrowser.Tag;

            if (node.Title == node.Url)
            {
                node.Title = webBrowser.DocumentTitle;
                var treeNode = FindTreeNodeByNodeId(node.Id);

                if (treeNode != null)
                {
                    treeNode.Text = node.Title;
                }

                SetDirty(false);

            }

            treeView1.Focus();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                var nodeTitle = e.Label;
                var node = (Node)e.Node.Tag;
                node.Title = nodeTitle;
                node.Url = null;
                SetDefaultDisplayType(node);
                SetDirty(true);
            }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Control)
            {
                ShowFindDialog();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3 && e.Shift)
            {
                var history = GetSearchHistory();
                FindPrev(history.FirstOrDefault());
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                var history = GetSearchHistory();
                FindNext(history.FirstOrDefault());
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Right && e.Control)
            {
                AddChildNode();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2)
            {
                RenameSelectedNode();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                var dialogResult = MessageBox.Show("Sure to this node?", "Warning", MessageBoxButtons.YesNoCancel);

                if (dialogResult == DialogResult.Yes)
                {
                    DeleteSelectedNode();
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter && treeView1.Focused)
            {
                ShowContent(treeView1.SelectedNode);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up && e.Control)
            {
                MoveNodeUp();
            }
            else if (e.KeyCode == Keys.Down && e.Control)
            {
                MoveNodeDown();
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ShowContent(e.Node);
        }

        private void ShowContent(TreeNode treeNode)
        {
            var node = (Node)treeNode.Tag;

            if (node == null)
            {
                return;
            }

            if (Control.ModifierKeys == Keys.Shift)
            {
                if (node.DisplayType == DisplayType.Browser)
                {
                    Process.Start(node.Url);
                    return;
                }
            }

            DisplayContent(node);
        }

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            UnhighlightTreeNodes();

            if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                e.Effect = DragDropEffects.Move;
                var point = new Point(e.X, e.Y);
                HighlightNodeAtPoint(point);
            }
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
                var point = new Point(e.X, e.Y);
                HighlightNodeAtPoint(point);
            }
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var pt = treeView1.PointToClient(new Point(e.X, e.Y));
                var destinationTreeNode = treeView1.GetNodeAt(pt);

                var treeNode = AddNode((Node)destinationTreeNode?.Tag);

                if (treeNode != null)
                {
                    var node = (Node)treeNode.Tag;
                    string nodeTitle = e.Data.GetData(DataFormats.Text).ToString();
                    node.Title = nodeTitle;
                    SetDefaultDisplayType(node);
                    ReloadTree();
                    SelectNodeById(node.Id);
                    SetDirty(false);
                }
            }
            else if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                var treeNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                var node = (Node)treeNode.Tag;
                var parentNode = (Node)treeNode.Parent?.Tag;
                var pt = treeView1.PointToClient(new Point(e.X, e.Y));
                var destinationTreeNode = treeView1.GetNodeAt(pt);
                var destinationNode = (Node)destinationTreeNode?.Tag;


                if (node.Id == destinationNode?.Id)
                {
                    // Drop on itself
                    return;
                }

                if (parentNode != null)
                {
                    parentNode.Nodes.Remove(node);
                }
                else
                {
                    _context.Nodes.Remove(node);
                }

                if (destinationNode != null)
                {
                    node.Index = destinationNode.Nodes.Any() ? destinationNode.Nodes.Last().Index + 1 : 0;
                    destinationNode.Nodes.Add(node);
                }
                else
                {
                    node.Index = _context.Nodes.Any() ? _context.Nodes.Last().Index + 1 : 0;
                    _context.Nodes.Add(node);
                }

                ReloadTree();

                SelectNodeById(node.Id);
                SetDirty(false);
            }
        }

        private void treeView1_DragLeave(object sender, EventArgs e)
        {
            UnhighlightTreeNodes();
        }

        private void expandButton_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void collapseButton_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void findMenuItem_Click(object sender, EventArgs e)
        {
            ShowFindDialog();
        }

        private void findNextMenuItem_Click(object sender, EventArgs e)
        {
            var history = GetSearchHistory();

            if (!history.Any())
            {
                ShowFindDialog();
            }
            else
            {
                FindNext(history.First());
            }
        }

        private void findPrevMenuItem_Click(object sender, EventArgs e)
        {
            var history = GetSearchHistory();

            if (!history.Any())
            {
                ShowFindDialog();
            }
            else
            {
                FindPrev(history.FirstOrDefault());
            }
        }

        private void moveNodeUpMenuItem_Click(object sender, EventArgs e)
        {
            MoveNodeUp();
        }

        private void moveNodeDownMenuItem_Click(object sender, EventArgs e)
        {
            MoveNodeDown();
        }

        private void MoveNodeDown()
        {
            var treeNode = treeView1.SelectedNode;
            if (treeNode == null) return;

            var nextTreeNode = treeNode.NextNode;
            if (nextTreeNode == null) return;

            var node1 = (Node)treeNode.Tag;
            var node2 = (Node)nextTreeNode.Tag;

            Swap(node1, node2);
        }

        private void MoveNodeUp()
        {
            var treeNode = treeView1.SelectedNode;
            if (treeNode == null) return;

            var prevTreeNode = treeNode.PrevNode;
            if (prevTreeNode == null) return;

            var node1 = (Node)prevTreeNode.Tag;
            var node2 = (Node)treeNode.Tag;

            Swap(node1, node2);
        }

        private void InitContextMenu()
        {
            _docMenu = new ContextMenuStrip();

            var addNodeMenuItem = new ToolStripMenuItem();
            addNodeMenuItem.Text = "Add Node";
            addNodeMenuItem.Click += delegate { AddChildNode(); };

            var deleteNodeMenuItem = new ToolStripMenuItem();
            deleteNodeMenuItem.Text = "Delete";
            deleteNodeMenuItem.Click += delegate { DeleteSelectedNode(); };

            var renameNodeMenuItem = new ToolStripMenuItem();
            renameNodeMenuItem.Text = "Rename";
            renameNodeMenuItem.Click += delegate { RenameSelectedNode(); };

            _docMenu.Items.AddRange(new ToolStripMenuItem[] { addNodeMenuItem, deleteNodeMenuItem, renameNodeMenuItem });
        }

        private TreeNode SelectNodeById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var treeNode = FindTreeNodeByNodeId(id);

            if (treeNode != null)
            {
                treeView1.SelectedNode = treeNode;
            }

            return treeNode;
        }

        private void RenameSelectedNode()
        {
            var treeNode = treeView1.SelectedNode;

            if (treeNode != null)
            {
                treeNode.BeginEdit();
            }
        }

        private void ReloadTree()
        {
            var nodeId = ((Node)treeView1.SelectedNode?.Tag)?.Id;

            treeView1.SuspendLayout();
            treeView1.Nodes.Clear();
            ReloadNodes(null, _context.Nodes);

            if (nodeId.HasValue)
            {
                SelectNodeById(nodeId.Value);
            }

            treeView1.ResumeLayout();
        }

        private void ReloadNodes(TreeNode parentNode, List<Node> nodes)
        {
            foreach (var node in nodes.OrderBy(n => n.Index))
            {
                var treeNode = new TreeNode { Text = node.Title, Tag = node };
                treeNode.ContextMenuStrip = _docMenu;

                if (parentNode != null)
                    parentNode.Nodes.Add(treeNode);
                else
                    treeView1.Nodes.Add(treeNode);

                if (node.Nodes.Any())
                {
                    ReloadNodes(treeNode, node.Nodes);
                }

                if (_context.Layout.ExpandedNodes.Contains(node.Id))
                {
                    treeNode.Expand();
                }

                ApplySpecialNodeStyles(treeNode);
            }
        }

        private void ApplySpecialNodeStyles(TreeNode treeNode)
        {
            var node = (Node)treeNode.Tag;

            if (node.DisplayType == DisplayType.Browser)
            {
                treeNode.ForeColor = Color.Blue;
            }
        }

        private bool SaveProject()
        {
            UpdateLayoutData(treeView1.Nodes);

            if (_currentProjectFilename == null)
            {
                var saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "Vision projects (*.visionproj)|*.visionproj";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                var dialogResult = saveFileDialog.ShowDialog();

                if (dialogResult == DialogResult.Cancel)
                {
                    return false;
                }

                if (dialogResult == DialogResult.OK)
                {
                    _currentProjectFilename = saveFileDialog.FileName;
                }
            }

            try
            {
                _persistor.Save(_context, _currentProjectFilename);
                ClearDirtyFlag();
            }
            catch
            {
                _currentProjectFilename = null;
            }

            return true;
        }

        private void UpdateLayoutData(TreeNodeCollection treeNodes)
        {
            _context.Layout.ExpandedNodes.Clear();

            UpdateLayoutDataRec(treeNodes);
        }

        private void UpdateLayoutDataRec(TreeNodeCollection treeNodes)
        {
            foreach (TreeNode treeNode in treeNodes)
            {
                if (treeNode.IsExpanded)
                {
                    var node = (Node)treeNode.Tag;

                    _context.Layout.ExpandedNodes.Add(node.Id);
                }

                UpdateLayoutDataRec(treeNode.Nodes);
            }
        }

        private void LoadLastProject()
        {
            var lastProjectFile = Properties.Settings.Default[Config.SETTINGS_LASTPROJECTFILE] as String;

            if (!string.IsNullOrWhiteSpace(lastProjectFile))
            {
                LoadProject(lastProjectFile);
            }
        }

        private void LoadProject(string fileName)
        {
            _context = _persistor.Load(fileName);
            _currentProjectFilename = fileName;
            Properties.Settings.Default[Config.SETTINGS_LASTPROJECTFILE] = fileName;
            Properties.Settings.Default.Save();
            ReloadTree();
            Text = Path.GetFileNameWithoutExtension(fileName);
        }

        private void AddChildNode()
        {
            var parentTreeNode = treeView1.SelectedNode;

            if (parentTreeNode != null)
            {
                var parentNode = (Node)parentTreeNode.Tag;
                var treeNode = AddNode(parentNode);
                if (treeNode != null)
                {
                    treeNode.BeginEdit();
                }
            }
        }

        private TreeNode AddNode(Node parentNode = null)
        {
            var node = _context.AddNode(parentNode, "New");
            UpdateLayoutData(treeView1.Nodes);
            ReloadTree();
            var treeNode = SelectNodeById(node.Id);
            SetDirty(true);
            return treeNode;
        }

        private void DeleteSelectedNode()
        {
            var treeNode = treeView1.SelectedNode;

            if (treeNode != null)
            {
                var node = (Node)treeNode.Tag;

                if (treeNode.Parent == null)
                {
                    _context.RemoveNode(null, node);
                }
                else
                {
                    var parentNode = (Node)treeNode.Parent.Tag;
                    _context.RemoveNode(parentNode, node);
                }

                SetDirty(true);

                var nodeToSelect = (treeNode.NextNode?.Tag) as Node ?? (treeNode.PrevNode?.Tag) as Node ?? (treeNode.Parent?.Tag) as Node;

                ReloadTree();

                if (nodeToSelect != null)
                {
                    SelectNodeById(nodeToSelect.Id);
                }
            }
        }

        private void SetDirty(bool contentChanged)
        {
            _dirty = true;

            if (contentChanged)
            {
                _modified = true;
            }
        }

        private void ClearDirtyFlag()
        {
            _dirty = false;
        }


        public void FindNext(string searchText)
        {
            AddToSearchHistory(searchText);
            var matches = FindMatchingNodes(searchText);

            if (!matches.Any()) return;

            var currentNodeIndex = matches.IndexOf(GetSelectedNodeId());
            var nextNodeGuid = matches[(currentNodeIndex + 1) % matches.Count];

            SelectNodeById(nextNodeGuid);
        }

        public void FindPrev(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return;
            }

            AddToSearchHistory(searchText);
            var matches = FindMatchingNodes(searchText);

            if (!matches.Any()) return;

            var currentNodeIndex = matches.IndexOf(GetSelectedNodeId());
            var nextNodeGuid = matches[(currentNodeIndex + matches.Count - 1) % matches.Count];

            SelectNodeById(nextNodeGuid);
        }

        public void BookmarkAll(string searchText)
        {
            AddToSearchHistory(searchText);
        }

        public void FindAll(string searchText)
        {
            AddToSearchHistory(searchText);
        }

        public string[] GetSearchHistory()
        {
            var obj = Properties.Settings.Default["SearchTextHistory"];

            if (obj == null)
            {
                Properties.Settings.Default["SearchTextHistory"] = new StringCollection();
                Properties.Settings.Default.Save();
                return new string[0];
            }

            var result = (obj as StringCollection).Cast<String>().ToArray();
            return result;
        }


        private void ShowFindDialog()
        {
            if (_findForm.Visible) return;

            _findForm.Show();
        }

        private Guid GetSelectedNodeId()
        {
            if (treeView1.SelectedNode == null)
                return Guid.Empty;

            var node = (Node)treeView1.SelectedNode.Tag;
            return node.Id;
        }

        private TreeNode FindTreeNodeByNodeId(Guid id, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
            {
                nodes = treeView1.Nodes;
            }

            foreach (TreeNode treeNode in nodes)
            {
                var node = (Node)treeNode.Tag;

                if (node.Id == id)
                {
                    return treeNode;
                }

                var match = FindTreeNodeByNodeId(id, treeNode.Nodes);

                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private List<Guid> FindMatchingNodes(string searchText, TreeNodeCollection nodes = null)
        {
            var result = new List<Guid>();

            if (nodes == null)
            {
                nodes = treeView1.Nodes;
            }

            foreach (TreeNode treeNode in nodes)
            {
                var node = (Node)treeNode.Tag;

                var isMatch = node.Title?.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                isMatch = isMatch || node.Url?.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                isMatch = isMatch || node.Content?.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;

                if (isMatch)
                {
                    result.Add(node.Id);
                }

                var subResult = FindMatchingNodes(searchText, treeNode.Nodes);
                result.AddRange(subResult);
            }

            return result;
        }

        private static void AddToSearchHistory(string searchText)
        {
            var history = Properties.Settings.Default["SearchTextHistory"] as StringCollection;

            if (history == null)
            {
                history = new StringCollection();
                Properties.Settings.Default["SearchTextHistory"] = history;
                Properties.Settings.Default.Save();
            }

            if (history.Count > 0 && history[0] == searchText) return;

            history.Insert(0, searchText);
            Properties.Settings.Default.Save();
        }

        private void Export()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;

                Export(fileName);
                Process.Start(saveFileDialog.FileName);
            }
        }

        private void Export(string filename)
        {
            BL.Export.ToTextFile(_context.Nodes, filename);
        }

        private void Swap(Node node1, Node node2)
        {
            var index1 = node1.Index;
            node1.Index = node2.Index;
            node2.Index = index1;

            SetDirty(true);

            ReloadTree();
        }

        private HashSet<TreeNode> _highlightedTreeNodes = new HashSet<TreeNode>();

        private void HighlightNodeAtPoint(Point point)
        {
            var pt = treeView1.PointToClient(point);
            var treeNode = treeView1.GetNodeAt(pt);

            if (treeNode != null)
            {
                if (_highlightedTreeNodes.Contains(treeNode))
                {
                    return;
                }

                treeNode.ForeColor = Color.Orange;

                lock (_highlightedTreeNodes)
                {
                    _highlightedTreeNodes.Add(treeNode);
                }
            }
        }

        private void UnhighlightTreeNodes()
        {
            lock (_highlightedTreeNodes)
            {
                treeView1.BeginUpdate();
                foreach (var treeNode in _highlightedTreeNodes)
                {
                    treeNode.ForeColor = Color.Black;
                }
                treeView1.EndUpdate();
                _highlightedTreeNodes.Clear();
            }
        }

        private void SelectNodeAtPoint(Point point)
        {
            var pt = treeView1.PointToClient(point);
            var treeNode = treeView1.GetNodeAt(pt);

            if (treeNode != null)
            {
                treeView1.SelectedNode = treeNode;
            }
        }

        private void SetDefaultDisplayType(Node node)
        {
            if (Regex.IsMatch(node.Title, RegularExpressions.URL))
            {
                node.DisplayType = DisplayType.Browser;

                if (node.Url == null)
                {
                    node.Url = node.Title;
                }
            }
            else
            {
                node.DisplayType = DisplayType.Folder;
                node.Url = null;
            }
        }

        #region InitializeComponent
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplorerToolWindow));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.expandButton = new System.Windows.Forms.Button();
            this.collapseButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findPrevToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToplevelNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSiblingNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveNodeDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeNodeUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.HideSelection = false;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(12, 59);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(616, 402);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // expandButton
            // 
            this.expandButton.Location = new System.Drawing.Point(12, 27);
            this.expandButton.Name = "expandButton";
            this.expandButton.Size = new System.Drawing.Size(91, 26);
            this.expandButton.TabIndex = 1;
            this.expandButton.Text = "Expand All";
            this.expandButton.UseVisualStyleBackColor = true;
            this.expandButton.Click += new System.EventHandler(this.expandButton_Click);
            // 
            // collapseButton
            // 
            this.collapseButton.Location = new System.Drawing.Point(109, 27);
            this.collapseButton.Name = "collapseButton";
            this.collapseButton.Size = new System.Drawing.Size(91, 26);
            this.collapseButton.TabIndex = 1;
            this.collapseButton.Text = "Collapse All";
            this.collapseButton.UseVisualStyleBackColor = true;
            this.collapseButton.Click += new System.EventHandler(this.collapseButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.contentToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(640, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.newProjectToolStripMenuItem.Text = "&New Project";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.saveToolStripMenuItem.Text = "&Save Project";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveFileMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.openProjectToolStripMenuItem.Text = "&Open Project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openFileMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.exportToolStripMenuItem.Text = "&Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportFileMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.findPrevToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(208, 26);
            this.findToolStripMenuItem.Text = "&Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(208, 26);
            this.findNextToolStripMenuItem.Text = "Find &Next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextMenuItem_Click);
            // 
            // findPrevToolStripMenuItem
            // 
            this.findPrevToolStripMenuItem.Name = "findPrevToolStripMenuItem";
            this.findPrevToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findPrevToolStripMenuItem.Size = new System.Drawing.Size(208, 26);
            this.findPrevToolStripMenuItem.Text = "Find &Prev";
            this.findPrevToolStripMenuItem.Click += new System.EventHandler(this.findPrevMenuItem_Click);
            // 
            // contentToolStripMenuItem
            // 
            this.contentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToplevelNodeToolStripMenuItem,
            this.addChildNodeToolStripMenuItem,
            this.addSiblingNodeToolStripMenuItem,
            this.modeNodeUpToolStripMenuItem,
            this.moveNodeDownToolStripMenuItem});
            this.contentToolStripMenuItem.Name = "contentToolStripMenuItem";
            this.contentToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.contentToolStripMenuItem.Text = "&Content";
            // 
            // addToplevelNodeToolStripMenuItem
            // 
            this.addToplevelNodeToolStripMenuItem.Name = "addToplevelNodeToolStripMenuItem";
            this.addToplevelNodeToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            this.addToplevelNodeToolStripMenuItem.Text = "Add &Toplevel Node";
            this.addToplevelNodeToolStripMenuItem.Click += new System.EventHandler(this.addToplevelNodeMenuItem_Click);
            // 
            // addChildNodeToolStripMenuItem
            // 
            this.addChildNodeToolStripMenuItem.Name = "addChildNodeToolStripMenuItem";
            this.addChildNodeToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            this.addChildNodeToolStripMenuItem.Text = "Add &Child Node";
            this.addChildNodeToolStripMenuItem.Click += new System.EventHandler(this.addChildNodeMenuItem_Click);
            // 
            // addSiblingNodeToolStripMenuItem
            // 
            this.addSiblingNodeToolStripMenuItem.Name = "addSiblingNodeToolStripMenuItem";
            this.addSiblingNodeToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            this.addSiblingNodeToolStripMenuItem.Text = "Add &Sibling Node";
            this.addSiblingNodeToolStripMenuItem.Click += new System.EventHandler(this.addSiblingNodeMenuItem_Click);
            // 
            // moveNodeDownToolStripMenuItem
            // 
            this.moveNodeDownToolStripMenuItem.Name = "moveNodeDownToolStripMenuItem";
            this.moveNodeDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.moveNodeDownToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            this.moveNodeDownToolStripMenuItem.Text = "Move Node &Down";
            this.moveNodeDownToolStripMenuItem.Click += new System.EventHandler(this.moveNodeDownMenuItem_Click);
            // 
            // modeNodeUpToolStripMenuItem
            // 
            this.modeNodeUpToolStripMenuItem.Name = "modeNodeUpToolStripMenuItem";
            this.modeNodeUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.modeNodeUpToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            this.modeNodeUpToolStripMenuItem.Text = "Mode Node &Up";
            this.modeNodeUpToolStripMenuItem.Click += new System.EventHandler(this.moveNodeUpMenuItem_Click);
            // 
            // ExplorerToolWindow
            // 
            this.ClientSize = new System.Drawing.Size(640, 473);
            this.Controls.Add(this.collapseButton);
            this.Controls.Add(this.expandButton);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ExplorerToolWindow";
            this.Text = "Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DockableExplorer_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion // InitializeComponent
    }
}
