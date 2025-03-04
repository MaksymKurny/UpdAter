using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using UpdAter.BL;

namespace UpdAter
{
    public partial class UaBlock : UserControl
    {
        public event EventHandler blockDeleted, blockChanged, needUpdate, changeCB;
        private Image deffBannerImage = null;
        private int borderRadius = 10;
        private bool newBlock = true;
        private Ukrainizer _ukrainizer;

        public UaBlock(Ukrainizer ukrainizer, bool noData)
        {
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            deffBannerImage = this.BackgroundImage;
            _ukrainizer = ukrainizer;
            if (!noData)
            {
                SetData();
                UpdateLastUpdate();
            }
        }
        public Ukrainizer GetUkrainizer()
        {
            return _ukrainizer;
        }
        public string GetId()
        {
            return _ukrainizer.Id;
        }
        public bool GetPinCheckbox()
        {
            return menuPin.Checked;
        }
        public (ProgressBar, Label) GetProgressBar()
        {
            return (progressBar, txtPercent);
        }

        public void SetData()
        {
            txtTitle.Text = _ukrainizer.Title;
            menuGuide.Visible = !String.IsNullOrWhiteSpace(_ukrainizer.GuideUrl);
            newBlock = false;
            menuAddToList.Checked = _ukrainizer.AddToList;
            menuPin.Checked = _ukrainizer.PinnedState;

            if (File.Exists(_ukrainizer.Banner))
            {
                if (this.BackgroundImage != Image.FromFile(_ukrainizer.Banner))
                {
                    this.BackgroundImage = Image.FromFile(_ukrainizer.Banner);
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

            if (File.Exists(_ukrainizer.Icon))
            {
                string extension = Path.GetExtension(_ukrainizer.Icon).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    gameIcon.Image = Image.FromFile(_ukrainizer.Icon);
                }
                else
                {
                    gameIcon.Image = Icon.ExtractAssociatedIcon(_ukrainizer.Icon).ToBitmap();
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

        private void menuGuide_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(_ukrainizer.GuideUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити посилання: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", _ukrainizer.Path);
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
            DialogResult result = MessageBox.Show("Ви впевнені, що бажаєте видалити переклад?", "Видалити", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                blockDeleted?.Invoke(this, EventArgs.Empty);
        }

        public void btnEdit_Click(object sender, EventArgs e)
        {
            UaForm editForm = new UaForm(_ukrainizer.GetEditedData());
            DialogResult result = editForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                _ukrainizer.SetEditedData(editForm.GetData());
                SetData();
                blockChanged?.Invoke(this, EventArgs.Empty);
            } else if (result == DialogResult.Cancel && newBlock)
            {
                blockDeleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateLastUpdate(bool update = false)
        {
            if (update) _ukrainizer.UpdateLastUpdate(DateTime.Now);
            if (_ukrainizer.LastUpdate == DateTime.MinValue) return;
            string date = _ukrainizer.LastUpdate.ToString("dd.MM.yyyy");
            string text = txtLastUpd.Text;
            string prefix = text.Substring(0, text.IndexOf(':'));
            txtLastUpd.Text = $"{prefix}: {date}";
        }

        private void menuButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem button)
            {
                if (button == menuPin)
                {
                    _ukrainizer.ChangePinnedState(button.Checked);
                }
                else if (button == menuAddToList)
                {
                    _ukrainizer.ChangeAddToList(button.Checked);
                }
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