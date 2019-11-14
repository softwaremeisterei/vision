using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vision.BL.Model;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for TilesControl.xaml
    /// </summary>
    public partial class TilesControl : UserControl
    {
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public class ViewModel : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;

            private ObservableCollection<NodeView> nodes;
            public ObservableCollection<NodeView> Nodes
            {
                get => nodes;
                set
                {
                    nodes = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nodes)));
                }
            }
        }

        private Project project;

        public ViewModel Model { get; set; }

        public delegate void LinkClickedHandler(NodeView nodeView);
        public event LinkClickedHandler LinkClicked;

        public TilesControl()
        {
            InitializeComponent();
            this.Model = new ViewModel();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var uiElement = sender as Border;
                var nodeView = uiElement.Tag as NodeView;
                LinkClicked?.Invoke(nodeView);
                e.Handled = true;
            }
        }

        public void Init(ObservableCollection<NodeView> nodes, Project project)
        {
            this.Model = new ViewModel
            {
                Nodes = nodes,
            };
            this.project = project;
            DataContext = this;
        }

        private void ContextMenuNode_Edit(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var nodeView = (NodeView)menuItem.Tag;
                Shared.EditNode(Window.GetWindow(this), nodeView);
                DataChanged?.Invoke(this, new EventArgs());
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
                var nodeView = Shared.AddNewNode(Window.GetWindow(this));
                Model.Nodes.Add(nodeView);
                project.Nodes.Add(nodeView.Tag as Node);
                DataChanged?.Invoke(this, new EventArgs());
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
                DataChanged?.Invoke(this, new EventArgs());
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
                Model.Nodes.Remove(nodeView);
                project.Nodes.Remove(nodeView.Tag as Node);
                DataChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void FindNext(string searchText)
        {
            throw new NotImplementedException();
        }
    }
}
