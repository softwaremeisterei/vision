using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vision.BL.Lib;
using Vision.BL.Model;
using Vision.Wpf.Mappers;
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

            private ObservableCollection<LinkView> links;
            public ObservableCollection<LinkView> Links
            {
                get => links;
                set
                {
                    links = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Links)));
                }
            }

            private ObservableCollection<LinkView> historyLinks;
            public ObservableCollection<LinkView> HistoryLinks
            {
                get => historyLinks;
                set
                {
                    historyLinks = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HistoryLinks)));
                }
            }

            private double tileSize = 160d;
            public double TileSize
            {
                get => tileSize;
                set
                {
                    tileSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TileSize)));
                }
            }

            private double fontSize = 16d;
            public double FontSize
            {
                get => fontSize;
                set
                {
                    fontSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FontSize)));
                }
            }

            private double smallTileSize = 80d;
            public double SmallTileSize
            {
                get => smallTileSize;
                set
                {
                    smallTileSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SmallTileSize)));
                }
            }

            private double smallFontSize = 8d;
            public double SmallFontSize
            {
                get => smallFontSize;
                set
                {
                    smallFontSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SmallFontSize)));
                }
            }
        }

        private Project project;

        public ViewModel Model { get; set; }

        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public delegate void LinkClickedHandler(LinkView linkView);
        public event LinkClickedHandler LinkClicked;

        public TilesControl()
        {
            InitializeComponent();
            this.Model = new ViewModel();

            InputBindings.Add(new KeyBinding(new ActionCommand(() =>
            {
                ToggleSearch();
            }), Key.F3, ModifierKeys.None));
        }

        public void Init(ObservableCollection<LinkView> linkViews, Project project)
        {
            this.Model = new ViewModel
            {
                Links = linkViews,
                HistoryLinks = new ObservableCollection<LinkView>(),
                TileSize = project.TileSize,
                FontSize = project.FontSize,
                SmallTileSize = project.SmallTileSize,
                SmallFontSize = project.SmallFontSize
            };
            this.project = project;
            DataContext = this;
        }

        private void ContextMenuLink_Edit(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var linkView = (LinkView)menuItem.Tag;
                Shared.EditLink(Window.GetWindow(this), linkView);
                DataChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleSearch()
        {
            if (!expFilter.IsExpanded)
            {
                expFilter.IsExpanded = true;
                tbKeywords.Focus();
                tbKeywords.SelectAll();
            }
            else
            {
                expFilter.IsExpanded = false;
            }
        }

        private void ContextMenu_AddLink(object sender, RoutedEventArgs e)
        {
            try
            {
                var linkView = Shared.AddNewLink(Window.GetWindow(this));
                Model.Links.Add(linkView);
                project.Links.Add(linkView.Tag as Link);
                DataChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuLink_ToggleFavorite(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var linkView = (LinkView)menuItem.Tag;
                Shared.ToggleFavorite(linkView);
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

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CkFavorites_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Model.Links.Clear();

            var links = new List<Link>(project.Links);


            if (ckFavorites.IsChecked ?? false)
            {
                links.RemoveAll(n => !n.IsFavorite);
            }

            var keywords = tbKeywords.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (keywords.Any())
            {
                links.RemoveAll(n => !keywords.Any(keyword => n.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            var linkViews = LinkMappers.MapToView(links.OrderBy(link => link.Name));

            foreach (var linkView in linkViews)
            {
                Model.Links.Add(linkView);
            }
        }

        private void SingleTileControl_LinkClicked(LinkView linkView)
        {
            Model.HistoryLinks.Remove(linkView);
            Model.HistoryLinks.Insert(0, linkView);
            while (Model.HistoryLinks.Count > 7)
            {
                var lastIndex = Model.HistoryLinks.Count - 1;
                Model.HistoryLinks.RemoveAt(lastIndex);
            }
            LinkClicked?.Invoke(linkView);
        }

        private void SingleTileControl_DeleteMe(object sender, EventArgs e)
        {
            try
            {
                var linkView = (sender as SingleTileControl).LinkView;
                Model.Links.Remove(linkView);
                project.Links.Remove(linkView.Tag as Link);
                DataChanged?.Invoke(this, new EventArgs());
                Model.HistoryLinks.Remove(linkView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddNewLink(string name, string url)
        {
            var link = new Link { Name = name, Url = url };
            project.Links.Add(link);
            Model.Links.Add(LinkMappers.MapToView(link));
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            var formats = e.Data.GetFormats();
            if (formats.Contains("Text"))
            {
                var text = e.Data.GetData("Text") as string;

                if (Regexes.URL.IsMatch(text))
                {
                    var linkView = Shared.AddNewLink(Window.GetWindow(this), text);
                    Model.Links.Add(linkView);
                    project.Links.Add(linkView.Tag as Link);
                    DataChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            const double deltaFactor = 1.10;

            if (e.Delta > 0)
            {
                Model.TileSize *= deltaFactor;
                Model.FontSize *= deltaFactor;
                Model.SmallTileSize *= deltaFactor;
                Model.SmallFontSize *= deltaFactor;
            }
            else if (e.Delta < 0)
            {
                Model.TileSize /= deltaFactor;
                Model.FontSize /= deltaFactor;
                Model.SmallTileSize /= deltaFactor;
                Model.SmallFontSize /= deltaFactor;
            }

            project.TileSize = Model.TileSize;
            project.FontSize = Model.FontSize;
            project.SmallTileSize = Model.SmallTileSize;
            project.SmallFontSize = Model.SmallFontSize;

            e.Handled = true;
        }
    }
}
