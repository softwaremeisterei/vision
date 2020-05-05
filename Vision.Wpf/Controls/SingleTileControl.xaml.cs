using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for SingleTileControl.xaml
    /// </summary>
    public partial class SingleTileControl : UserControl, INotifyPropertyChanged
    {
        public delegate void LinkClickedHandler(LinkView linkView);
        public event LinkClickedHandler LinkClicked;

        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public delegate void DeleteMeEventHandler(object sender, EventArgs e);
        public event DeleteMeEventHandler DeleteMe;

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty LinkViewProperty =
            DependencyProperty.Register("LinkView", typeof(LinkView), typeof(SingleTileControl), new PropertyMetadata(default(LinkView)));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(int), typeof(SingleTileControl), new PropertyMetadata(default(int)));
        
        private LinkViewService linkViewService;

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public int ImageSize { get { return (int)GetValue(SizeProperty) / 12; } set { } }

        public LinkView LinkView
        {
            get { return (LinkView)GetValue(LinkViewProperty); }
            set { SetValue(LinkViewProperty, value); }
        }

        public SingleTileControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
            linkViewService = new LinkViewService();
        }

        private void ContextMenu_Edit(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var linkView = (LinkView)menuItem.Tag;
                linkViewService.EditLink(Window.GetWindow(this), linkView);
                DataChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenu_Delete(object sender, RoutedEventArgs e)
        {
            DeleteMe?.Invoke(this, new EventArgs());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var linkView = button.Tag as LinkView;
            LinkClicked?.Invoke(linkView);
        }
    }
}
