using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision.Controls
{
    public partial class DragTextBox : TextBox
    {
        private const long WM_LBUTTONDOWN = 0x201;
        private string _dragText;

        public DragTextBox()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                _dragText = SelectedText;
            }

            base.WndProc(ref m);
        }

        private void DragTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(_dragText))
            {
                SelectionStart = Text.IndexOf(_dragText);
                SelectionLength = _dragText.Length;
                DoDragDrop(_dragText, DragDropEffects.Copy);
                SelectionLength = 0;
            }
        }
    }
}
