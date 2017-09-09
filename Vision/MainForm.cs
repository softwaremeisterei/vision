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
    public partial class MainForm : Form, ISearchable
    {
        Timer _timer = new Timer();
        Context _context;
        Persistor _persistor;
        private bool _dirty = false;
        private static bool _modified = false;
        private string _currentProjectFilename;
        FindForm _findForm;
        private ContextMenuStrip _docMenu;
        private bool _ignoreTextChange;
        private RichTextBoxExtended.RichTextBoxExtended _contentRichTextBox;
        private WebBrowser _contentWebBrowser;

        public MainForm()
        {
            InitializeComponent();

            InitializeContentControls();

            _context = new Context();
            _persistor = new Persistor();
            _findForm = new Forms.FindForm(this);

            InitContextMenu();
            InitDragDrop();
            LoadLastProject();
        }

        private void InitializeContentControls()
        {
            _contentRichTextBox = new RichTextBoxExtended.RichTextBoxExtended();
            _contentWebBrowser = new WebBrowser();

            _contentRichTextBox.Visible = false;
            _contentRichTextBox.Dock = DockStyle.Fill;
            _contentRichTextBox.RichTextBox.TextChanged += contentRichTextBox_TextChanged;
            splitContainer1.Panel2.Controls.Add(_contentRichTextBox);

            _contentWebBrowser.Visible = false;
            _contentWebBrowser.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(_contentWebBrowser);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void addToplevelNodeMenuItem_Click(object sender, EventArgs e)
        {
            var treeNode = AddNode();
            treeNode.BeginEdit();
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
                treeNode.BeginEdit();
            }
            else if (treeView1.SelectedNode != null)
            {
                var parentNode = (Node)treeView1.SelectedNode.Parent?.Tag;
                var treeNode = AddNode(parentNode);
                treeNode.BeginEdit();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _contentWebBrowser.Url = null;

            var node = (Node)treeView1.SelectedNode.Tag;

            try
            {
                _ignoreTextChange = true;
                DisplayContent(node);
            }
            finally
            {
                _ignoreTextChange = false;
            }

            _context.Layout.SelectedNode = node.Id;
            SetDirty(false);
        }

        private void DisplayContent(Node node)
        {
            foreach (Control control in splitContainer1.Panel2.Controls)
            {
                control.Visible = false;
            }

            switch (node.DisplayType)
            {
                case DisplayType.RichText:
                    _contentRichTextBox.Visible = true;
                    _contentRichTextBox.RichTextBox.Rtf = node.Content;
                    break;
                case DisplayType.Browser:
                    _contentWebBrowser.Visible = true;
                    _contentWebBrowser.Url = new Uri(node.Title);
                    break;
            }
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                var nodeTitle = e.Label;
                var node = (Node)e.Node.Tag;
                node.Title = nodeTitle;
                SetDefaultDisplayType(node);
                SetDirty(true);
            }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right && e.Control)
            {
                AddChildNode();
            }
            else if (e.KeyCode == Keys.F2)
            {
                RenameSelectedNode();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                var dialogResult = MessageBox.Show("Sure to this node?", "Warning", MessageBoxButtons.YesNoCancel);

                if (dialogResult == DialogResult.Yes)
                {
                    DeleteSelectedNode();
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (Regex.IsMatch(e.Node.Text, RegularExpressions.URL))
            {
                var url = e.Node.Text;
                ((Node)e.Node.Tag).DisplayType = DisplayType.Browser;
                SetDirty(true);
                Process.Start(url);
            }
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
            if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                e.Effect = DragDropEffects.Move;
                var point = new Point(e.X, e.Y);
                SelectNodeAtPoint(point);
            }
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
                var point = new Point(e.X, e.Y);
                SelectNodeAtPoint(point);
            }
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var pt = treeView1.PointToClient(new Point(e.X, e.Y));
                var destinationTreeNode = treeView1.GetNodeAt(pt);

                var treeNode = AddNode((Node)destinationTreeNode?.Tag);
                var node = (Node)treeNode.Tag;
                string nodeTitle = e.Data.GetData(DataFormats.Text).ToString();
                node.Title = nodeTitle;
                SetDefaultDisplayType(node);
                ReloadTree();
                SelectNodeById(node.Id);
            }
            else if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                var treeNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                var node = (Node)treeNode.Tag;
                var parentNode = (Node)treeNode.Parent?.Tag;
                var pt = treeView1.PointToClient(new Point(e.X, e.Y));
                var destinationTreeNode = treeView1.GetNodeAt(pt);
                var destinationNode = (Node)destinationTreeNode?.Tag;

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
            }
        }

        private void contentRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_ignoreTextChange) return;

            var selectedTreeNode = treeView1.SelectedNode;

            if (selectedTreeNode != null)
            {
                var node = (Node)selectedTreeNode.Tag;
                node.Content = _contentRichTextBox.RichTextBox.Rtf;
                SetDirty(true);
            }
        }

        private void expandButton_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
            SetDirty(false);
        }

        private void collapseButton_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
            SetDirty(false);
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
                FindPrev(history.First());
            }
        }

        private void exportMenuItem_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void moveNodeDownMenuItem_Click(object sender, EventArgs e)
        {
            var treeNode = treeView1.SelectedNode;
            if (treeNode == null) return;

            var nextTreeNode = treeNode.NextNode;
            if (nextTreeNode == null) return;

            var node1 = (Node)treeNode.Tag;
            var node2 = (Node)nextTreeNode.Tag;

            Swap(node1, node2);
        }

        private void moveNodeUpMenuItem_Click(object sender, EventArgs e)
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

        private void InitDragDrop()
        {
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(treeView_ItemDrag);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(treeView_DragEnter);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(treeView_DragOver);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(treeView_DragDrop);
        }

        private TreeNode SelectNodeById(Guid id)
        {
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
            treeView1.SuspendLayout();
            treeView1.Nodes.Clear();
            ReloadNodes(null, _context.Nodes);
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

                if (node.Id == _context.Layout.SelectedNode)
                {
                    treeView1.SelectedNode = treeNode;
                }

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
            if (Regex.IsMatch(treeNode.Text, RegularExpressions.URL))
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
        }

        private void AddChildNode()
        {
            var parentTreeNode = treeView1.SelectedNode;

            if (parentTreeNode != null)
            {
                var parentNode = (Node)parentTreeNode.Tag;
                var treeNode = AddNode(parentNode);
                treeNode.BeginEdit();
            }
        }

        private TreeNode AddNode(Node parentNode = null)
        {
            var node = _context.AddNode(parentNode, "New");
            UpdateLayoutData(treeView1.Nodes);
            SetDirty(true);
            ReloadTree();
            var treeNode = SelectNodeById(node.Id);
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

                if (node.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || node.Content.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
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

        private void SelectNodeAtPoint(Point point)
        {
            var pt = treeView1.PointToClient(point);
            var destinationTreeNode = treeView1.GetNodeAt(pt);

            if (destinationTreeNode != null)
            {
                treeView1.SelectedNode = destinationTreeNode;
            }
        }

        private void SetDefaultDisplayType(Node node)
        {
            if (Regex.IsMatch(node.Title, RegularExpressions.URL))
            {
                node.DisplayType = DisplayType.Browser;
            }
            else
            {
                node.DisplayType = DisplayType.RichText;
            }
        }
    }
}
