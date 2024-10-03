using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdAter.BL;

namespace UpdAter
{
    public partial class MainForm : Form
    {
        private Bl BL;
        private int borderRadius = 8; // Рівень заокруглення
        private GDownloader GDownloader;

        //Пересування форми
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            using (GraphicsPath path = g.GenerateRoundedRectangle(this.ClientRectangle, borderRadius))
            {
                g.FillPath(new SolidBrush(this.BackColor), path);
                this.Region = new Region(path);
            }
         }

        public MainForm(string[] args)
        {
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            BL = new Bl();
            GDownloader = new GDownloader();
            UpdateList();
            AutoUpdate(args);
            updateAll.Click += async (s, ev) => await GDownloader.DownloadFilesAsync(BL.ukrainizers.List, uaList);
        }

        private async void AutoUpdate(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("--game="))
                {
                    string gameTitles = arg.Substring(7).Trim('"');
                    await GDownloader.AutoDownloadFilesAsync(BL.ukrainizers.List, uaList, gameTitles);
                    if (this.ShowInTaskbar == false)
                    {
                        await Task.Delay(1000);
                        Application.Exit();
                    }
                }
            }
        }

        private void btn_addNew_Click(object sender, EventArgs e)
        {
            AddNewBlock(new Ukrainizer(), true);
        }

        private void AddNewBlock(Ukrainizer block, bool newBlock = true)
        {
            UaBlock uaBlock = new UaBlock(block.GetData());
            uaBlock.Dock = DockStyle.Top;
            uaBlock.blockDeleted += blockDeleted;
            uaBlock.needUpdate += blockUpdate;
            uaBlock.blockChanged += blockChanged;

            uaList.Controls.Add(uaBlock);

            if (newBlock)
            {
                uaList.Controls.SetChildIndex(uaBlock, 0);
                BL.ukrainizers.List.Insert(0, block);
                uaBlock.btnEdit_Click(this, EventArgs.Empty);
            }
        }

        private async void blockUpdate(object sender, EventArgs e)
        {
            if (sender is UaBlock uaBlock)
            {
                uaBlock.enabledButtons(false);
                await GDownloader.DownloadFileAsync(uaBlock.GetUrl(), uaBlock.GetPath(), uaBlock.GetProgressBar());
                uaBlock.enabledButtons(true);
            }
        }

        private void blockChanged(object sender, EventArgs e)
        {
            if (sender is UaBlock uaBlock)
            {
                int index = uaList.Controls.IndexOf(uaBlock);
                if (index == -1) return;
                BL.ukrainizers.UpdateUkrainizer(index, uaBlock.GetData());
            }

            BL.saveSettings();
        }

        private void blockDeleted(object sender, EventArgs e)
        {
            if (sender is UaBlock uaBlock)
            {
                BL.ukrainizers.DellUkrainizer(uaBlock.GetTitle(), uaBlock.GetUrl());
                uaList.Controls.Remove(uaBlock);
                BL.saveSettings();
            }
        }

        public void UpdateList()
        {
            uaList.Controls.Clear();

            foreach (var ukrainizer in BL.ukrainizers.List)
                AddNewBlock(ukrainizer, false);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void credits_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }
    }
}
