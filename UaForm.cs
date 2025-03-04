using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace UpdAter
{
    public partial class UaForm : Form
    {
        //Пересування форми
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void UaForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        public UaForm((string url, string path, string title, string iconPath, string bannerPath, string guideUrl) data)
        {
            InitializeComponent();

            titleTextBox.Text = data.title;
            gamePathTextBox.Text = data.path;
            urlTextBox.Text = data.url;
            iconTextBox.Text = data.iconPath;
            bannerTextBox.Text = data.bannerPath;
            guideTextBox.Text = data.guideUrl;

            helpToolTip.SetToolTip(helpUrl, "Пряме посилання на файл (Google drive/GitHub)\nПодробиці у README файлі");
            helpToolTip.SetToolTip(helpPath, "Тека, в яку буде завантажено файли");
            helpToolTip.SetToolTip(helpGuide, "Посилання на посібник для швидкого відкриття");

            this.urlTextBox.TextChanged += new System.EventHandler(this.CheckFieldsFilled);
            this.titleTextBox.TextChanged += new System.EventHandler(this.CheckFieldsFilled);
            this.iconTextBox.TextChanged += new System.EventHandler(this.CheckFieldsFilled);
            this.bannerTextBox.TextChanged += new System.EventHandler(this.CheckFieldsFilled);
            this.guideTextBox.TextChanged += new System.EventHandler(this.CheckFieldsFilled);
            this.gamePathTextBox.TextChanged += new System.EventHandler(this.gamePathTextBox_TextChanged);

            gamePathTextBox_TextChanged(this, EventArgs.Empty);
        }

        public (string, string, string, string, string, string) GetData()
        {
            return (
                urlTextBox.Text,
                gamePathTextBox.Text,
                titleTextBox.Text,
                iconTextBox.Text,
                bannerTextBox.Text,
                guideTextBox.Text
            );
        }

        private void OpenFolderBrowser(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Оберіть папку з грою";

                if (!string.IsNullOrEmpty(gamePathTextBox.Text) && Directory.Exists(gamePathTextBox.Text))
                {
                    folderDialog.SelectedPath = gamePathTextBox.Text;
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    gamePathTextBox.Text = selectedPath;
                }
            }
        }

        private void UpdateGameInfo(string basePath)
        {
            (string steamAppsPath, string gameFolderName) = GetSteamAppsPath(basePath, "steamapps", "common");
            if (steamAppsPath == string.Empty && gameFolderName == string.Empty) return;

            (string name, string banner, string icon) = GetAppInfoFromACFFile(steamAppsPath, gameFolderName);
            if (name != null)
            {
                if (titleTextBox.Text == "") titleTextBox.Text = name;
                if (bannerTextBox.Text == "") bannerTextBox.Text = banner;
                if (iconTextBox.Text == "") iconTextBox.Text = icon;
            }
        }

        private (string, string) GetSteamAppsPath(string currentPath, string searchDirectory, string parrentGameDirectory)
        {
            string[] pathParts = currentPath.Split(Path.DirectorySeparatorChar);
            int index = Array.IndexOf(pathParts, searchDirectory);
            int parrentIndex = Array.IndexOf(pathParts, parrentGameDirectory);

            if (index == -1 || parrentIndex == -1 || parrentIndex + 1 >= pathParts.Length)
            {
                return (string.Empty, string.Empty);
            }
            return (string.Join(Path.DirectorySeparatorChar.ToString(), pathParts.Take(index + 1)), pathParts[parrentIndex + 1]);
        }

        private (string, string) GetSteamMedia(string currentPath, string searchDirectory, string id)
        {

            string[] pathParts = currentPath.Split(Path.DirectorySeparatorChar);
            int index = Array.IndexOf(pathParts, searchDirectory);

            if (index == -1) return (null, null);
            string libPath = string.Join(Path.DirectorySeparatorChar.ToString(), pathParts.Take(index + 1));
            libPath = Path.Combine(libPath, "appcache", "librarycache", id);

            // Шлях до банера
            string bannerPath = Path.Combine(libPath, "library_hero.jpg");
            if (!File.Exists(bannerPath))
            {
                // Якщо банер не знайдено, шукаємо у підкаталогах
                var bannerDirs = Directory.GetDirectories(libPath);
                bannerPath = bannerDirs
                    .Select(dir => Path.Combine(dir, "library_hero.jpg"))
                    .FirstOrDefault(File.Exists);
            }

            // Пошук іконки
            string iconFile = Directory.GetFiles(libPath, "*.jpg")
                                        .FirstOrDefault(file => Path.GetFileNameWithoutExtension(file).Length == 40);

            return (bannerPath, iconFile);
        }

        private (string, string, string) GetAppInfoFromACFFile(string directoryPath, string targetDir)
        {
            if (directoryPath != "")
            {
                // Рекурсивно проходимо всі файли в директорії
                foreach (string filePath in Directory.GetFiles(directoryPath, "appmanifest_*.acf"))
                {
                    string fileContent = File.ReadAllText(filePath);

                    var dirMatch = Regex.Match(fileContent, @"\""installdir\""[\s\t]*\""(.+?)\""", RegexOptions.Singleline);
                    if (dirMatch.Success && dirMatch.Groups[1].Value.Equals(targetDir, StringComparison.OrdinalIgnoreCase))
                    {
                        // Шукаємо `appid` і `name` та інше в секції "AppState"
                        var appidMatch = Regex.Match(fileContent, @"\""appid\""[\s\t]*\""(\d+)\""", RegexOptions.Singleline);
                        var nameMatch = Regex.Match(fileContent, @"\""name\""[\s\t]*\""(.+?)\""", RegexOptions.Singleline);
                        var steamFolderMatch = Regex.Match(fileContent, @"\""LauncherPath\""[\s\t]*\""(.+?)\""", RegexOptions.Singleline);
                        if (nameMatch.Success && appidMatch.Success && steamFolderMatch.Success)
                        {
                            string name = nameMatch.Groups[1].Value;
                            string appid = appidMatch.Groups[1].Value;
                            string steamFolder = steamFolderMatch.Groups[1].Value;
                            (string banner, string icon) = GetSteamMedia(steamFolder, "Steam", appid);

                            return (name, banner, icon);
                        }
                    }
                }
            }

            return (null, null, null);
        }


        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _) && (url.StartsWith("http") || url.StartsWith("https"));
        }

        private bool IsValidPath(string path)
        {
            return Directory.Exists(path);
        }

        private void CheckFieldsFilled(object sender, EventArgs e)
        {
            bool allFieldsFilled = IsValidUrl(urlTextBox.Text)
                                && IsValidPath(gamePathTextBox.Text)
                                && !string.IsNullOrWhiteSpace(titleTextBox.Text);

            btnOk.Enabled = allFieldsFilled;
        }

        private void btnIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(iconTextBox.Text) && File.Exists(iconTextBox.Text))
                {
                    fileDialog.InitialDirectory = Path.GetDirectoryName(iconTextBox.Text);
                }
                fileDialog.Filter = "Піктограма файли(*.ico *.icl *.exe *.dll)|*.ico; *.icl; *.exe; *.dll|Зображення(*.png *.jpg)|*.png; *.jpg|Всі файли(*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    iconTextBox.Text = fileDialog.FileName;
                }
            }
        }

        private void btnBaner_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(bannerTextBox.Text) && File.Exists(bannerTextBox.Text))
                {
                    fileDialog.InitialDirectory = Path.GetDirectoryName(bannerTextBox.Text);
                }

                fileDialog.Filter = "Image files(*.png *.jpg)|*.png; *.jpg|All files(*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    bannerTextBox.Text = fileDialog.FileName;
                }
            }
        }

        private void gamePathTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckFieldsFilled(sender, e);
            string path = gamePathTextBox.Text;
            btnFillInfo.Enabled = (path.Contains("steamapps") && path.Contains("common") && IsValidPath(path));
        }

        private void btnFillInfo_Click(object sender, EventArgs e)
        {
            UpdateGameInfo(gamePathTextBox.Text);
        }
    }
}
