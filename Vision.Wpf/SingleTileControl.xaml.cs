using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for SingleTileControl.xaml
    /// </summary>
    public partial class SingleTileControl : UserControl
    {
        public delegate void LinkClickedHandler(NodeView nodeView);
        public event LinkClickedHandler LinkClicked;

        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public delegate void DeleteMeEventHandler(object sender, EventArgs e);
        public event DeleteMeEventHandler DeleteMe;

        public static readonly DependencyProperty NodeViewProperty = 
            DependencyProperty.Register("NodeView", typeof(NodeView), typeof(SingleTileControl), new PropertyMetadata(default(NodeView)));

        public static readonly DependencyProperty SizeProperty = 
            DependencyProperty.Register("Size", typeof(int), typeof(SingleTileControl), new PropertyMetadata(default(int)));

        public int Size {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public int RelativeFontSize { get { return Size / 9; } set { } }

        public int RelativeImageSize { get { return Size / 7; } set { } }

        public NodeView NodeView
        {
            get { return (NodeView)GetValue(NodeViewProperty); }
            set { SetValue(NodeViewProperty, value); }
        }

        public SingleTileControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
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
            DeleteMe?.Invoke(this, new EventArgs());
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
    }
}
