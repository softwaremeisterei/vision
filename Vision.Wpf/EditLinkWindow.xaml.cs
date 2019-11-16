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
        { }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Global.Mapper.Map(LinkView, LinkView.Tag as Link);
            DialogResult = true;
        }

    }
}
