using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private string bannerPath;
        private string url;
        private Image bannerImage;
        public UaBlock()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(MainForm_Paint);
            resources = new ResourceManager(typeof(UaBlock));
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
        public string GetBanner()
        {
            return bannerPath;
        }
        public string GetIcon()
        {
            return iconPath;
        }
        public (ProgressBar, Label) GetProgressBar()
        {
            return (progressBar, txtPercent);
        }

        // Метод для встановлення тексту блоку
        public void SetData((string title, string url, string path, string icon, string banner) data)
        {
            txtTitle.Text = data.title;
            path = data.path;
            url = data.url;
            iconPath = data.icon;
            bannerPath = data.banner;
            if (File.Exists(iconPath))
            {
                gameIcon.Image = Icon.ExtractAssociatedIcon(iconPath).ToBitmap();
            }
            else
            {
                gameIcon.Image = null;
            }
            if (File.Exists(bannerPath))
            {
                bannerImage = Image.FromFile(bannerPath);
                this.Invalidate(); // Викликає перерисовку контролю
            }
            else
            {
                this.BackgroundImage = null;
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (bannerImage != null)
            {
                // Налаштування банера
                float scaleFactor = Math.Min((float)this.ClientSize.Width / bannerImage.Width, (float)this.ClientSize.Height / bannerImage.Height);
                int scaledWidth = (int)(bannerImage.Width * scaleFactor);
                int scaledHeight = (int)(bannerImage.Height * scaleFactor);

                e.Graphics.DrawImage(bannerImage, new Rectangle(0, 0, scaledWidth, scaledHeight));

                // Налаштування градієнту
                int gradientWidth = 100;
                Rectangle gradientRect = new Rectangle(this.ClientSize.Width - 50 - gradientWidth, 0, gradientWidth, this.ClientSize.Height);

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    gradientRect,
                    Color.Transparent,
                    this.BackColor,
                    LinearGradientMode.Horizontal))
                {
                    Blend blend = new Blend();
                    blend.Positions = new float[] { 0f, 0.75f, 1f };
                    blend.Factors = new float[] { 0f, 1f, 1f };

                    brush.Blend = blend;

                    e.Graphics.FillRectangle(brush, gradientRect);
                }
            }
        }

        public (string, string, string, string, string) GetData()
        {
            return (GetTitle(), GetPath(), GetUrl(), GetIcon(), GetBanner());
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
            UaForm editForm = new UaForm(GetData());

            // Показати форму редагування модально
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Оновити нотатку в NoteBlock
                this.SetData(editForm.GetData());
                blockChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            needUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
}