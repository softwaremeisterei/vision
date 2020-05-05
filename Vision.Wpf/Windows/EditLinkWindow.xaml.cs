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
using System.Windows.Shapes;
using Vision.BL.Model;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for Prompt.xaml
    /// </summary>
    public partial class EditLinkWindow : Window
    {
        public LinkView LinkView { get; set; }

        public EditLinkWindow(LinkView linkView)
        {
            InitializeComponent();
            LinkView = linkView;
            DataContext = LinkView;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbUrl.Focus();
            tbUrl.SelectAll();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Global.Mapper.Map(LinkView, LinkView.Tag as Link);
            DialogResult = true;
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PromptWindow("Add tag");
            dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (dlg.ShowDialog() == true)
            {
                LinkView.Tags.Add(dlg.ResponseText);
            }
        }

        private void EditTag_Click(object sender, RoutedEventArgs e)
        {
            var index = lbTags.SelectedIndex;
            if (index >= 0)
            {
                var dlg = new PromptWindow("Edit tag", null, lbTags.Items[index].ToString());
                dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (dlg.ShowDialog() == true)
                {
                    LinkView.Tags[index] = dlg.ResponseText;
                }
            }
        }

        private void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            var index = lbTags.SelectedIndex;
            if (index >= 0)
            {
                LinkView.Tags.RemoveAt(index);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
