using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision.Forms
{
    public partial class FindForm : Form
    {
        ISearchable _client;

        public FindForm(ISearchable client)
        {
            _client = client;
            InitializeComponent();
        }

        private void FindForm_Load(object sender, EventArgs e)
        {
            string[] searchTextHistory = _client.GetSearchHistory() ?? new string[0];

            searchTextComboBox.Items.AddRange(searchTextHistory);

            if (searchTextHistory.Any())
            {
                searchTextComboBox.Text = searchTextHistory.First();
            }

            searchTextComboBox.Focus();
            searchTextComboBox.SelectAll();
        }

        private void findPrevButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchTextComboBox.Text))
            {
                _client.FindPrev(searchTextComboBox.Text);
            }

            searchTextComboBox.Focus();
            searchTextComboBox.SelectAll();
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchTextComboBox.Text))
            {
                _client.FindNext(searchTextComboBox.Text);
            }

            searchTextComboBox.Focus();
            searchTextComboBox.SelectAll();
        }

        private void bookmarkAllButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchTextComboBox.Text))
            {
                _client.BookmarkAll(searchTextComboBox.Text);
            }
        }

        private void findAllButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchTextComboBox.Text))
            {
                Visible = false;
                _client.FindAll(searchTextComboBox.Text);
            }
        }

        private void FindForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void searchTextComboBox_TextUpdate(object sender, EventArgs e)
        {
            Enable();
        }

        private void Enable()
        {
            var searchTextAvailable = !string.IsNullOrEmpty(searchTextComboBox.Text);
            findPrevButton.Enabled = searchTextAvailable;
            findNextButton.Enabled = searchTextAvailable;
            bookmarkAllButton.Enabled = searchTextAvailable;
            findAllButton.Enabled = searchTextAvailable;
        }

        private void searchTextComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Hide();
            }
        }
    }
}
