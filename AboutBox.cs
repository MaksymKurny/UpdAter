using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdAter
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void default_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender is LinkLabel link)
            {
                System.Diagnostics.Process.Start(link.Text);
            }
        }

        private async void CheckForUpdates(object sender, EventArgs e)
        {
            string currentVersion = "1.0.0";
            string repoOwner = "MaksymKurny";
            string repoName = "UpdAter";

            string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");  // GitHub вимагає UserAgent

                    // Запит до GitHub API
                    var response = await client.GetStringAsync(apiUrl);
                    JObject release = JObject.Parse(response);

                    // Отримуємо останню версію релізу
                    string latestVersion = release["tag_name"].ToString();  // Тег версії релізу

                    if (latestVersion != currentVersion)
                    {
                        string downloadUrl = release["assets"][0]["browser_download_url"].ToString();  // URL для завантаження

                        // Завантаження оновлення
                        DownloadUpdate(downloadUrl, "update.zip");

                        MessageBox.Show($"Знайдено нову версію {latestVersion}. Завантаження оновлення...");
                    }
                    else
                    {
                        MessageBox.Show("У вас вже встановлена остання версія.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час перевірки оновлень: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DownloadUpdate(string url, string fileName)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    // Запис завантаженого файлу
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }

                    // Виконуємо після завантаження
                    MessageBox.Show("Оновлення завантажено. Розпаковка...");
                    ExtractZipFile(fileName, "update_folder");  // Виклик функції для розпаковки
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час завантаження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtractZipFile(string zipFilePath, string extractPath)
        {
            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                File.Delete(zipFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час розпаковки архіву: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
