using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace UpdAter
{
    public partial class UaBlock : UserControl
    {
        public event EventHandler blockDeleted, blockChanged, needUpdate, changeCB;
        private string id, path, iconPath, bannerPath, url;
        private Image deffBannerImage = null;
        private int borderRadius = 10;
        private DateTime LastUpdate;
        private bool newBlock = true;

        public UaBlock((string id, string, string, string, string, string, DateTime, bool, bool) data, bool noData)
        {
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            deffBannerImage = this.BackgroundImage;
            if (!noData)
            {
                SetData(data);
                UpdateLastUpdate();
            } else
            {
                id = data.id;
            }
        }
        public string GetId()
        {
            return id;
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
        private string GetBanner()
        {
            return bannerPath;
        }
        private string GetIcon()
        {
            return iconPath;
        }
        public DateTime GetLastUpdate()
        {
            return LastUpdate;
        }
        public bool GetListCheckbox()
        {
            return menuAddToList.Checked;
        }
        public bool GetPinCheckbox()
        {
            return menuPin.Checked;
        }
        public bool IsNew()
        {
            return newBlock;
        }
        public (ProgressBar, Label) GetProgressBar()
        {
            return (progressBar, txtPercent);
        }
        public (string, string, string, string, string, DateTime, bool, bool) GetFullData()
        {
            return (GetTitle(), GetPath(), GetUrl(), GetIcon(), GetBanner(), GetLastUpdate(), GetListCheckbox(), GetPinCheckbox());
        }

        public (string, string, string, string, string) GetData()
        {
            return (GetTitle(), GetPath(), GetUrl(), GetIcon(), GetBanner());
        }

        public void SetShortData((string title, string url, string path, string icon, string banner) data)
        {
            SetData((id, data.title, data.url, data.path, data.icon, data.banner, LastUpdate, GetListCheckbox(), GetPinCheckbox()));
        }

        public void SetData((string id, string title, string url, string path, 
            string icon, string banner, DateTime lastUpdate, bool updateAll, bool pinnedState) data)
        {
            id = data.id;
            txtTitle.Text = data.title;
            path = data.path;
            url = data.url;
            iconPath = data.icon;
            bannerPath = data.banner;
            newBlock = false;
            LastUpdate = data.lastUpdate;
            menuAddToList.Checked = data.updateAll;
            menuPin.Checked = data.pinnedState;

            if (File.Exists(bannerPath))
            {
                if (this.BackgroundImage != Image.FromFile(bannerPath))
                {
                    this.BackgroundImage = Image.FromFile(bannerPath);
                    txtTitle.BackgroundColor = Color.FromArgb(89, 0, 0, 0);
                    txtLastUpd.BackgroundColor = Color.FromArgb(89, 0, 0, 0);
                    this.Invalidate();
                }
            }
            else
            {
                txtTitle.BackgroundColor = Color.Transparent;
                txtLastUpd.BackgroundColor = Color.Transparent;
                this.BackgroundImage = deffBannerImage;
                this.Invalidate();
            }
            if (File.Exists(iconPath))
            {
                string extension = Path.GetExtension(iconPath).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    gameIcon.Image = Image.FromFile(iconPath);
                }
                else
                {
                    gameIcon.Image = Icon.ExtractAssociatedIcon(iconPath).ToBitmap();
                }
                gameIcon.Visible = true;
                txtTitle.Location = new Point(gameIcon.Right + 10, 12);
            }
            else
            {
                gameIcon.Image = null;
                gameIcon.Visible = false;
                if (this.BackgroundImage == null)
                {
                    txtTitle.Location = new Point(12, 12);
                } else
                {
                    txtTitle.Location = new Point(17, 12);
                }
                
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.FillRectangle(new SolidBrush(Color.FromArgb(27, 40, 56)), this.ClientRectangle);

            // Налаштування прямокутника для фону
            using (GraphicsPath path = g.GenerateRoundedRectangle(this.ClientRectangle, borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillRoundedRectangle(brush, 0, 0, this.Width, this.Height, borderRadius);
                }

                // Встановлюємо обрізку в межах заокругленого прямокутника
                g.SetClip(path);
                if (this.BackgroundImage != null)
                {
                    // Налаштування банера
                    float scaleFactor = Math.Min((float)this.ClientSize.Width / this.BackgroundImage.Width, (float)this.ClientSize.Height / this.BackgroundImage.Height);
                    int scaledWidth = (int)(this.BackgroundImage.Width * scaleFactor);
                    int scaledHeight = (int)(this.BackgroundImage.Height * scaleFactor);
                    g.DrawImage(this.BackgroundImage, new Rectangle(0, 1, scaledWidth, this.ClientSize.Height - 2 ));
                    if (this.BackgroundImage != deffBannerImage)
                    {
                        // Налаштування градієнту
                        int gradientWidth = 100;
                        Rectangle gradientRect = new Rectangle(scaledWidth + 1 - gradientWidth, 0, gradientWidth, this.ClientSize.Height);

                        using (LinearGradientBrush brush = new LinearGradientBrush(
                            gradientRect,
                            Color.Transparent,
                            this.BackColor,
                            LinearGradientMode.Horizontal))
                        {
                            Blend blend = new Blend();
                            blend.Positions = new float[] { 0f, 0.5f, 1f };
                            blend.Factors = new float[] { 0f, 0.5f, 1f };
                            brush.Blend = blend;

                            g.FillRectangle(brush, gradientRect);
                        }
                    }
                }
                g.DrawRoundedRectangle(new Pen(Color.FromArgb(43, 45, 86), 6), 0, 0, this.Width, this.Height, borderRadius);
            }
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            moreMenu.Show(Cursor.Position);
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити папку: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void enabledButtons(bool status)
        {
            btnMore.Enabled = status;
            btnUpdate.Enabled = status;
        }

        private void btnDell_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Ви спевнені що бажаєте видалити переклад?", "Видалити", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                blockDeleted?.Invoke(this, EventArgs.Empty);
        }

        public void btnEdit_Click(object sender, EventArgs e)
        {
            UaForm editForm = new UaForm(GetData());
            DialogResult result = editForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.SetShortData(editForm.GetData());
                blockChanged?.Invoke(this, EventArgs.Empty);
            } else if (result == DialogResult.Cancel && newBlock)
            {
                blockDeleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateLastUpdate(bool update = false)
        {
            if (update) LastUpdate = DateTime.Now;
            if (LastUpdate == DateTime.MinValue) return;
            string date = LastUpdate.ToString("dd.MM.yyyy");
            string text = txtLastUpd.Text;
            string prefix = text.Substring(0, text.IndexOf(':'));
            txtLastUpd.Text = $"{prefix}: {date}";
        }

        private void menuButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem button)
            {
                changeCB?.Invoke(this, new ButtonChangedEventArgs(button.Name));
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            needUpdate?.Invoke(this, EventArgs.Empty);
        }
    }

    public class ButtonChangedEventArgs : EventArgs
    {
        public string ButtonName { get; }
        public ButtonChangedEventArgs(string buttonName)
        {
            ButtonName = buttonName;
        }
    }
}