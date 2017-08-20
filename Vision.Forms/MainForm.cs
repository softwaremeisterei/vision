using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.BL;
using Vision.BL.Model;

namespace Vision.Forms
{
    public partial class MainForm : Form
    {
        Context _context;
        Persistor _persistor;
        private bool _dirty;
        private string _currentProjectFilename;

        public MainForm()
        {
            Init();

            InitializeComponent();

            LoadLastProject();
        }

        private void Init()
        {
            _context = new Context();
            _persistor = new Persistor();
        }

        private void LoadLastProject()
        {
            var lastProjectFile = Properties.Settings.Default[Config.SETTINGS_LASTPROJECTFILE] as String;

            if (!string.IsNullOrWhiteSpace(lastProjectFile))
            {
                LoadProject(lastProjectFile);
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

        private void LoadProject(string fileName)
        {
            _context = _persistor.Load(fileName);
            _currentProjectFilename = fileName;
            Properties.Settings.Default[Config.SETTINGS_LASTPROJECTFILE] = fileName;
            Properties.Settings.Default.Save();
            ReloadTree();
        }

        private void addNodeButton_Click(object sender, EventArgs e)
        {
            AddNewNode();
        }

        private void addNewNodeMenuItem_Click(object sender, EventArgs e)
        {
            AddNewNode();
        }

        private void AddNewNode(Node parentNode = null)
        {
            _context.AddNode(parentNode, "New");
            SetDirty();
            ReloadTree();
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
            foreach (var node in nodes)
            {
                var treeNode = new TreeNode { Text = node.Title, Tag = node };

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
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = (Node)treeView1.SelectedNode.Tag;
            contentRichTextBox.Text = node.Content;
            _context.Layout.SelectedNode = node.Id;
        }

        private void contentRichTextBox_TextChanged(object sender, EventArgs e)
        {
            var selectedTreeNode = treeView1.SelectedNode;

            if (selectedTreeNode != null)
            {
                var node = (Node)selectedTreeNode.Tag;
                node.Content = contentRichTextBox.Text;
            }

            SetDirty();
        }

        private void SetDirty()
        {
            _dirty = true;
        }

        private void ClearDirtyFlag()
        {
            _dirty = false;
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
        private bool SaveProject()
        {
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

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            var node = (Node)e.Node.Tag;
            node.Title = e.Label;
            SetDirty();
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right && e.Control)
            {
                AddNewNodeUnder();
            }
            else if (e.KeyCode == Keys.F2)
            {
                var treeNode = treeView1.SelectedNode;

                if (treeNode != null)
                {
                    treeNode.BeginEdit();
                }
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

        private void AddNewNodeUnder()
        {
            var treeNode = treeView1.SelectedNode;
            if (treeNode != null)
            {
                var node = (Node)treeNode.Tag;
                AddNewNode(node);
            }
        }

        private void DeleteSelectedNode()
        {
            var treeNode = treeView1.SelectedNode;

            if (treeNode != null)
            {
                var node = (Node)treeNode.Tag;

                if (treeNode.Parent == null)
                    _context.RemoveNode(null, node);
                else
                {
                    var parentNode = (Node)treeNode.Parent.Tag;
                    _context.RemoveNode(parentNode, node);
                }
                SetDirty();
                ReloadTree();
            }
        }

        private void addNodeUnderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewNodeUnder();
        }
    }
}
