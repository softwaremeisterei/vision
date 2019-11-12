using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public class ViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private NodeView parentNodeView;
            public NodeView ParentNodeView
            {
                get => parentNodeView;
                set
                {
                    parentNodeView = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParentNodeView)));
                }
            }

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
            if (e.LeftButton == MouseButtonState.Pressed)
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
                    LinkClicked?.Invoke(nodeView);
                }

                e.Handled = true;
            }
        }

        public void Init(NodeView root, List<BreadcrumbView> breadcrumbs = null)
        {
            this.Model = new ViewModel
            {
                ParentNodeView = root,
                Breadcrumbs = new ObservableCollection<BreadcrumbView>(breadcrumbs ?? new[] { new BreadcrumbView(root) }.ToList())
            };
            ReplaceRoot(root);
            DataContext = this;
        }

        private void ReplaceRoot(NodeView rootNodeView)
        {
            Model.ParentNodeView = rootNodeView;
        }

        private void ContextMenuNode_Edit(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var nodeView = (NodeView)menuItem.Tag;
                Shared.EditNode(Window.GetWindow(this), nodeView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenu_AddNode(object sender, RoutedEventArgs e)
        {
            try
            {
                Shared.AddNewNode(Window.GetWindow(this), Model.ParentNodeView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenu_AddFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                Shared.AddNewFolder(Window.GetWindow(this), Model.ParentNodeView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuNode_ToggleFavorite(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var nodeView = (NodeView)menuItem.Tag;
                Shared.ToggleFavorite(nodeView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuNode_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var nodeView = (NodeView)menuItem.Tag;
                var parentNodeView = GetParentFolder();
                Shared.DeleteNode(parentNodeView, nodeView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private NodeView GetParentFolder()
        {
            return Model.Breadcrumbs.Last().NodeView;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.XButton1 == MouseButtonState.Pressed)
            {
                if (Model.Breadcrumbs.Count > 1)
                {
                    Model.Breadcrumbs.Remove(Model.Breadcrumbs.Last());
                    ReplaceRoot(Model.Breadcrumbs.Last().NodeView);
                }

                e.Handled = true;
            }
        }
    }
}
