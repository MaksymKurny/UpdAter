using System;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;

namespace UpdAter
{
    public partial class UaBlock : UserControl
    {
        public event EventHandler blockDeleted;
        public event EventHandler blockChanged;
        public event EventHandler needUpdate;
        private ResourceManager resources;
        private string path;
        private string iconPath;
        private string url;
        public UaBlock()
        {
            InitializeComponent();
            resources = new ResourceManager(typeof(UaBlock));
        }

        // Метод для встановлення тексту блоку
        public void SetInfo(string title, string _url, string _path, string _iconPath)
        {
            txtTitle.Text = title;
            path = _path;
            url = _url;
            if (File.Exists(_iconPath))
            {
                iconPath = _iconPath;
                gameIcon.Image = Icon.ExtractAssociatedIcon(_iconPath).ToBitmap();
            }
        }

        public string GetTitle()
        {
            return txtTitle.Text;
        }

        public string GetUrl()
        {
            return url;
        }

        public string GetPath()
        {
            return path;
        }

        public string GetIcon()
        {
            return iconPath;
        }

        public ProgressBar GetProgressBar()
        {
            return progressBar;
        }

        public void enabledButtons(bool status)
        {
            btnEdit.Enabled = status;
            btnUpdate.Enabled = status;
            btnDell.Enabled = status;
        }

        private void btnDell_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(resources.GetString("btnDell.Message"), resources.GetString("btnDell.MessageTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                blockDeleted?.Invoke(this, EventArgs.Empty);
        }

        public void btnEdit_Click(object sender, EventArgs e)
        {
            // Створення нової форми для редагування
            UaForm editForm = new UaForm(txtTitle.Text, path, url, iconPath);

            // Показати форму редагування модально
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Отримати оновлені дані з форми редагування
                string updatedTitle = editForm.GetTitle();
                string updatedUrl = editForm.GetUrl();
                string updatedPath = editForm.GetPath();
                string updatedIcon = editForm.GetIcon();

                // Оновити нотатку в NoteBlock
                this.SetInfo(updatedTitle, updatedUrl, updatedPath, updatedIcon);
                blockChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            needUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
}