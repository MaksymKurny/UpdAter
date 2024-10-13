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
        private int pinnedCount;

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
            pinnedCount = 0;
            UpdateList();
            if( args.Length > 0)
            {
                AutoUpdate(args);
            }
        }

        private async void AllUpdate(object sender, EventArgs e)
        {
            if (BL.ukrainizers.List.Count > 0 && BL.ukrainizers.HasUaInList())
            {
                await GDownloader.DownloadFilesAsync(BL.ukrainizers.List, uaList);
            }
            else
            {
                MessageBox.Show($"Спершу додайте переклади у список!", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
            UaBlock uaBlock = new UaBlock(block.GetData(), newBlock);
            uaBlock.Dock = DockStyle.Top;
            uaBlock.blockDeleted += blockDeleted;
            uaBlock.needUpdate += blockUpdate;
            uaBlock.blockChanged += blockChanged;
            uaBlock.changeCB += blockChangeCb;

            uaList.Controls.Add(uaBlock);

            if (newBlock)
            {
                uaList.Controls.SetChildIndex(uaBlock, pinnedCount);
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
                uaBlock.UpdateLastUpdate(true);

                BL.ukrainizers.UpdateUkrainizerDate(uaBlock.GetId(), uaBlock.GetLastUpdate());
                BL.saveSettings();
            }
        }

        private void blockChangeCb(object sender, EventArgs e)
        {
            if (e is ButtonChangedEventArgs buttonArgs)
            {
                if (sender is UaBlock uaBlock)
                {
                    if (buttonArgs.ButtonName == "menuPin")
                    {
                        string id = uaBlock.GetId();
                        bool isCheked = uaBlock.GetPinCheckbox();
                        BL.ukrainizers.ChangePinnedState(id, isCheked);
                        var (pinned, unpinned) = BL.ukrainizers.GetPinnedList();
                        if (isCheked)
                        {
                            pinnedCount++;
                            int i = pinned.FindIndex(u => u.Id == id);
                            uaList.Controls.SetChildIndex(uaBlock, i != -1 ? i : 0);
                            
                        }
                        else if (!isCheked)
                        {
                            pinnedCount--;
                            int i = unpinned.FindIndex(u => u.Id == id);
                            uaList.Controls.SetChildIndex(uaBlock, pinnedCount + (i != -1 ? i : 0));
                        }
                    }
                    else if (buttonArgs.ButtonName == "menuAddToList")
                    {
                        BL.ukrainizers.ChangeAddToList(uaBlock.GetId(), uaBlock.GetListCheckbox());
                    }
                    BL.saveSettings();
                }
            }

        }

        private void blockChanged(object sender, EventArgs e)
        {
            if (sender is UaBlock uaBlock)
            {
                BL.ukrainizers.UpdateUkrainizer(uaBlock.GetId(), uaBlock.GetFullData());
                BL.saveSettings();
            }
        }

        private void blockDeleted(object sender, EventArgs e)
        {
            if (sender is UaBlock uaBlock)
            {
                BL.ukrainizers.DellUkrainizer(uaBlock.GetId());
                uaList.Controls.Remove(uaBlock);
                BL.saveSettings();
            }
        }

        public void UpdateList()
        {
            uaList.Controls.Clear();

            var (pinned, unpinned) = BL.ukrainizers.GetPinnedList();
            pinnedCount = pinned.Count;

            foreach (var ukrainizer in pinned)
                AddNewBlock(ukrainizer, false);

            foreach (var ukrainizer in unpinned)
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
