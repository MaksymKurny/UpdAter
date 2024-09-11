using System;
using System.Windows.Forms;
using UpdAter.BL;

namespace UpdAter
{
    public partial class MainForm : Form
    {
        private Bl BL;
        private GDownloader GDownloader;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            BL = new Bl();
            GDownloader = new GDownloader();
            UpdateList();
            updateAll.Click += async (s, ev) => await GDownloader.DownloadFilesAsync(BL.ukrainizers.List, uaList);
        }

        private void btn_addNew_Click(object sender, EventArgs e)
        {
            AddNewBlock(new Ukrainizer(), true);
        }

        private void AddNewBlock(Ukrainizer block, bool newBlock = true)
        {
            UaBlock uaBlock = new UaBlock();
            uaBlock.SetInfo(block.Title, block.Url, block.Path, block.Icon);
            uaBlock.Dock = DockStyle.Top;
            uaBlock.blockDeleted += blockDeleted;
            uaBlock.needUpdate += blockUpdate;
            uaBlock.blockChanged += blockChanged;

            uaList.Controls.Add(uaBlock);

            if (newBlock)
            {
                uaList.Controls.SetChildIndex(uaBlock, 0);
                BL.ukrainizers.List.Insert(0, new Ukrainizer(uaBlock.GetTitle(), uaBlock.GetPath(), uaBlock.GetUrl()));
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
                BL.ukrainizers.UpdateUkrainizer(index, uaBlock.GetTitle(), uaBlock.GetPath(), uaBlock.GetUrl(), uaBlock.GetIcon());
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
    }
}
