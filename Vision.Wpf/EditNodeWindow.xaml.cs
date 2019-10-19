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

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for Prompt.xaml
    /// </summary>
    public partial class EditNodeWindow : Window
    {
        public Node Node { get; set; }

        public EditNodeWindow(Node node)
        {
            InitializeComponent();
            DataContext = this;

            Node = node;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        { }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}
