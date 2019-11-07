using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for TilesControl.xaml
    /// </summary>
    public partial class TilesControl : UserControl
    {
        public class ViewModel {
            public ObservableCollection<NodeView> Nodes { get; set; }
            public ObservableCollection<BreadcrumbView> Breadcrumbs { get; set; }
        }

        public ViewModel Model { get; set; }

        public delegate void LinkClickedHandler(NodeView nodeView);
        public event LinkClickedHandler LinkClicked;

        public TilesControl()
        {
            InitializeComponent();
            this.Model = new ViewModel();
        }

        private void Breadcrumb_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            var breadcrumb = hyperlink.Tag as BreadcrumbView;
            var index = Model.Breadcrumbs.IndexOf(breadcrumb);
            ReplaceRoot(breadcrumb.NodeView);

            while (Model.Breadcrumbs.Count > index + 1)
            {
                Model.Breadcrumbs.RemoveAt(Model.Breadcrumbs.Count - 1);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var uiElement = sender as Border;
            var nodeView = uiElement.Tag as NodeView;

            if (nodeView.NodeType == NodeViewType.Folder)
            {
                Model.Breadcrumbs.Add(new BreadcrumbView(nodeView));
                ReplaceRoot(nodeView);
            }
            else if (nodeView.NodeType == NodeViewType.Link)
            {
                if (LinkClicked != null)
                {
                    LinkClicked(nodeView);
                }
            }
        }

        public void Init(NodeView root, List<BreadcrumbView> breadcrumbs = null)
        {
            this.Model = new ViewModel
            {
                Nodes = new ObservableCollection<NodeView>(),
                Breadcrumbs = new ObservableCollection<BreadcrumbView>(breadcrumbs ?? new[] { new BreadcrumbView(root) }.ToList())
            };
            ReplaceRoot(root);
            DataContext = Model;
        }

        private void ReplaceRoot(NodeView rootNodeView)
        {
            Model.Nodes.Clear();

            foreach (var childNodeView in rootNodeView.Nodes)
            {
                Model.Nodes.Add(childNodeView);
            }
        }

    }
}
