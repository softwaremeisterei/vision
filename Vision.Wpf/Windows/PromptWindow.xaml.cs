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

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for Prompt.xaml
    /// </summary>
    public partial class PromptWindow : Window
    {
        private readonly string _title;
        private readonly string _label;

        public PromptWindow(string title = null, string label = null, string value = null)
        {
            InitializeComponent();
            _title = title;
            _label = label;
            if (value != null)
            {
                tbResponse.Text = value;
            }
        }

        public string ResponseText
        {
            get { return tbResponse.Text; }
            set { tbResponse.Text = value; }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_title != null)
            {
                Title = _title;
            }

            if (_label != null)
            {
                tbHint.Text = _label;
            }

            tbResponse.Focus();
        }
    }
}
