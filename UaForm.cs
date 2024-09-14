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
        public UaForm((string title, string path, string url, string iconPath, string bannerPath) data)
        {
            InitializeComponent();

            titleTextBox.Text = data.title;
            gamePathTextBox.Text = data.path;
            urlTextBox.Text = data.url;
            iconTextBox.Text = data.iconPath;
            bannerTextBox.Text = data.bannerPath;
        }

        public (string, string, string, string, string) GetData()
        {
            return (
                titleTextBox.Text,
                urlTextBox.Text, 
                gamePathTextBox.Text, 
                iconTextBox.Text, 
                bannerTextBox.Text
            );
        }

        private void OpenFolderBrowser(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Оберіть папку з грою";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    gamePathTextBox.Text = selectedPath;
                    UpdateGameInfo(selectedPath);
                }
            }
        }

        private void UpdateGameInfo(string basePath)
        {
            string steamAppsPath = GetSteamAppsPath(basePath, "steamapps");
            string gameFolderName = GetSteamAppsPath(basePath, "common", true);

            (string name, string banner, string icon) = GetAppInfoFromACFFile(steamAppsPath, gameFolderName);
            if (name != null)
            {
                if (titleTextBox.Text == "") titleTextBox.Text = name;
                if (bannerTextBox.Text == "") bannerTextBox.Text = banner;
                if (iconTextBox.Text == "") iconTextBox.Text = icon;
            }
        }

        private (string, string) GetMedia(string basePath, string id)
        {
            string bannerPath = basePath + $"\\appcache\\librarycache\\{id}_library_hero.jpg";
            string iconPath = basePath + $"\\appcache\\librarycache\\{id}_icon.jpg";
            if (!bannerPath.Contains(@":\"))
            {
                bannerPath = bannerPath.Replace(":", @":\");
                iconPath = iconPath.Replace(":", @":\");
            }
            return (File.Exists(bannerPath) ? bannerPath : null,
                File.Exists(iconPath) ? iconPath : null);
        }

        private string GetSteamAppsPath(string currentPath, string searchDirectory, bool nextFolderName = false)
        {

            // Розділяємо шлях на частини
            string[] pathParts = currentPath.Split(Path.DirectorySeparatorChar);

            // Знаходимо індекс папки steamapps
            int index = Array.IndexOf(pathParts, searchDirectory);

            if (index >= 0)
            {
                // Будуємо шлях до steamapps
                if (nextFolderName && index + 2 <= pathParts.Length)
                {
                    return pathParts[index + 1];
                } else
                {
                    return Path.Combine(pathParts.Take(index + 1).ToArray());
                } 
            }
            return "";
        }

        private (string, string, string) GetAppInfoFromACFFile(string directoryPath, string targetInstalldir)
        {
            if (directoryPath != "")
            {
                // Рекурсивно проходимо всі файли в директорії
                foreach (string filePath in Directory.GetFiles(directoryPath, "*.acf", SearchOption.AllDirectories))
                {
                    string fileContent = File.ReadAllText(filePath);

                    // Шукаємо `appid` і `name` в секції "AppState"
                    var appidMatch = Regex.Match(fileContent, @"\""appid\""[\s\t]*\""(\d+)\""", RegexOptions.Singleline);
                    var nameMatch = Regex.Match(fileContent, @"\""name\""[\s\t]*\""(.+?)\""", RegexOptions.Singleline);
                    var installdirMatch = Regex.Match(fileContent, @"\""installdir\""[\s\t]*\""(.+?)\""", RegexOptions.Singleline);
                    var steamFolder = Regex.Match(fileContent, @"\""LauncherPath\""[\s\t]*\""(.+?)\""", RegexOptions.Singleline);

                    if (installdirMatch.Success && nameMatch.Success && appidMatch.Success)
                    {
                        string installdir = installdirMatch.Groups[1].Value;
                        if (installdir.Equals(targetInstalldir, StringComparison.OrdinalIgnoreCase))
                        {
                            string appid = appidMatch.Groups[1].Value;
                            string name = nameMatch.Groups[1].Value;
                            (string banner, string icon) = GetMedia(GetSteamAppsPath(steamFolder.Groups[1].Value, "Steam"), appidMatch.Groups[1].Value);

                            return (name, banner, icon);
                        }
                    }
                }
            }
            
            return (null, null, null);
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

        private void btnBaner_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "Image files(*.png *.jpg)|*.png; *.jpg|All files(*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    bannerTextBox.Text = fileDialog.FileName;
                }
            }
        }

        private void gamePathTextBox_TextChanged(object sender, EventArgs e)
        {
            if (gamePathTextBox.Text != "")
            {
                UpdateGameInfo(gamePathTextBox.Text);
            }
        }
    }
}
