using System;
using System.Drawing;
using System.Windows.Forms;

namespace UpdAter
{
    public partial class UaForm : Form
    {
        public UaForm(string title, string path, string url, string iconPath)
        {
            InitializeComponent();

            titleTextBox.Text = title;
            gamePathTextBox.Text = path;
            urlTextBox.Text = url;
            iconTextBox.Text = iconPath;
        }
        public string GetTitle()
        {
            return titleTextBox.Text;
        }
        public string GetUrl()
        {
            return urlTextBox.Text;
        }
        public string GetPath()
        {
            return gamePathTextBox.Text;
        }
        public string GetIcon()
        {
            return iconTextBox.Text;
        }

        private void OpenFolderBrowser(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Оберіть папку з грою";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    gamePathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "Exe files(*.exe)|*.exe|Ico files(*.ico)|*.ico|All files(*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    iconTextBox.Text = fileDialog.FileName;
                }
            }
        }
    }
}
