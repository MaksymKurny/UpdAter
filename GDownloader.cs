using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdAter.BL;

namespace UpdAter
{
    class GDownloader
    {
        public async Task DownloadFilesAsync(List<Ukrainizer> ukrainizers, TableLayoutPanel uaList)
        {
            List<Task> downloadTasks = new List<Task>();
            int index = 0;
            foreach (var ukrainizer in ukrainizers)
            {
                UaBlock uaBlock = (UaBlock)uaList.Controls[index++];
                if (uaBlock != null)
                {
                    uaBlock.enabledButtons(false);
                    downloadTasks.Add(DownloadFileAsync(ukrainizer.Url, ukrainizer.Path, uaBlock.GetProgressBar()));
                }
            }

            try
            {
                await Task.WhenAll(downloadTasks);
                index = 0;
                foreach (var ukrainizer in ukrainizers)
                {
                    UaBlock uaBlock = (UaBlock)uaList.Controls[index++];
                    if (uaBlock != null)
                    {
                        uaBlock.enabledButtons(true);
                    }
                }
                MessageBox.Show("Оновлення завершено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час оновлення перекладу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task AutoDownloadFilesAsync(List<Ukrainizer> ukrainizers, TableLayoutPanel uaList, string gameNamesArray)
        {
            string[] gameNames = gameNamesArray.Split(';');
            List<Task> downloadTasks = new List<Task>();
            int index = 0;
            foreach (var ukrainizer in ukrainizers)
            {
                if (gameNames.Contains(ukrainizer.Title))
                {
                    UaBlock uaBlock = (UaBlock)uaList.Controls[index++];
                    if (uaBlock != null)
                    {
                        uaBlock.enabledButtons(false);
                        downloadTasks.Add(DownloadFileAsync(ukrainizer.Url, ukrainizer.Path, uaBlock.GetProgressBar()));
                    }
                }
                else
                {
                    index++;
                }  
            }

            try
            {
                await Task.WhenAll(downloadTasks);
                index = 0;
                foreach (var ukrainizer in ukrainizers)
                {
                    UaBlock uaBlock = (UaBlock)uaList.Controls[index++];
                    if (uaBlock != null)
                    {
                        uaBlock.enabledButtons(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час оновлення перекладу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task DownloadFileAsync(string url, string path, (ProgressBar progressBar, Label percentLabel) block)
        {
            try
            {
                url = ProcessGoogleDriveUrl(url);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                    // Перевірка, чи потрібне підтвердження
                    if (response.Content.Headers.ContentType.MediaType == "text/html")
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        string confirmLink = "";
                        // Перевіряємо на наявність підтвердження
                        if (content.Contains("uc-warning-caption"))
                        {
                            confirmLink = ExtractConfirmLink(content);

                        }
                        else if (content.Contains("property=\"og:url\""))
                        {
                            string googleUrl = Regex.Match(content, "property=\"og:url\" content=\"([^\"]+)\"").Groups[1].Value;
                            confirmLink = ProcessGoogleDriveUrl(googleUrl);
                            url = confirmLink;
                        }
                        if (!string.IsNullOrEmpty(confirmLink))
                        {
                            // Завантажуємо з підтвердженням
                            response = await client.GetAsync(confirmLink, HttpCompletionOption.ResponseHeadersRead);
                        }
                        else
                        {
                            throw new Exception("Не вдалося знайти посилання для підтвердження.");
                        }
                    }

                    response.EnsureSuccessStatusCode();

                    string fileName = GetFileNameFromUrlOrResponse(url, response);
                    string fullPath = Path.Combine(path, fileName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                await fileStream.WriteAsync(buffer, 0, read);
                                totalRead += read;

                                if (totalBytes != -1)
                                {
                                    var progress = (int)((totalRead * 1.0 / totalBytes) * 100);
                                    block.progressBar.Value = progress;
                                    block.percentLabel.Visible = true;
                                    block.percentLabel.Text = progress.ToString() + '%';
                                }
                            }
                        }
                        while (isMoreToRead);
                    }

                    if (fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        ExtractZipFile(fullPath, path);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час оновлення перекладу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ExtractConfirmLink(string htmlContent)
        {
            string uuid = Regex.Match(htmlContent, "name=\"uuid\" value=\"([^\"]+)\"").Groups[1].Value;
            string fileId = Regex.Match(htmlContent, "name=\"id\" value=\"([^\"]+)\"").Groups[1].Value;

            if (string.IsNullOrEmpty(uuid) || string.IsNullOrEmpty(fileId))
            {
                throw new Exception("Не вдалося витягти uuid або id з HTML.");
            }

            return $"https://drive.usercontent.google.com/download?id={fileId}&confirm=t&uuid={uuid}";
        }

        private string GetFileNameFromUrlOrResponse(string url, HttpResponseMessage response)
        {
            string fileName = Path.GetFileName(new Uri(url).AbsolutePath);
            System.Console.WriteLine(response);
            if (string.IsNullOrEmpty(fileName) || fileName == "uc")
            {
                if (response.Content.Headers.ContentDisposition != null)
                {
                    fileName = response.Content.Headers.ContentDisposition.FileName.Trim('"');
                }
                else
                {
                    fileName = "ukrainizer.html";
                }
            }
            return fileName;
        }

        private void ExtractZipFile(string zipFilePath, string extractPath)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(extractPath, entry.FullName);

                        // Перевіряємо чи файл вже існує
                        if (File.Exists(destinationPath))
                        {
                            File.Delete(destinationPath); // Видаляємо існуючий файл перед розпаковкою
                        }

                        // Якщо це директорія, то створюємо її, інакше - розпаковуємо файл
                        if (entry.Name == "")
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            // Створюємо директорію, якщо її ще немає
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                            entry.ExtractToFile(destinationPath);
                        }
                    }
                }
                File.Delete(zipFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час розпакування архіву: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ProcessGoogleDriveUrl(string url)
        {
            var match = Regex.Match(url, @"https:\/\/drive\.google\.com\/file\/d\/([^\/]+)\/view");
            if (match.Success)
            {
                string fileId = match.Groups[1].Value;
                url = $"https://drive.google.com/uc?export=download&id={fileId}";
            }
            return url;
        }
    }
}
