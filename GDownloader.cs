using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Common;
using UpdAter.BL;

namespace UpdAter
{
    class GDownloader
    {
        private UaBlock FindBlockById(TableLayoutPanel uaList, string id)
        {
            foreach (Control control in uaList.Controls)
            {
                if (control is UaBlock uaBlock && uaBlock.GetId() == id)
                {
                    return uaBlock;
                }
            }
            return null;
        }

        private async Task<DateTime> GetLatestCommitDateFromGitHub(string url)
        {
            (string repoName, string branchName) = ExtractRepoNameFromUrl(url);
            string commitsUrl = $"https://api.github.com/repos/{repoName}/commits?sha={branchName}&per_page=1";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                HttpResponseMessage response = await client.GetAsync(commitsUrl);
                response.EnsureSuccessStatusCode();

                string json = response.Content.ReadAsStringAsync().Result;
                dynamic commits = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                if (commits == null || commits.Count == 0)
                {
                    throw new Exception("Не вдалося знайти коміти у вказаній гілці.");
                }
                return DateTime.Parse(commits[0].commit.committer.date.ToString());
            }
        }

        private (string, string) ExtractRepoNameFromUrl(string url)
        {
            var segments = new Uri(url).Segments;
            if (segments.Length >= 6)
            {
                return ($"{segments[1]}{segments[2].TrimEnd('/')}", segments[6].TrimEnd('.', 'z', 'i', 'p'));
            }
            throw new ArgumentException("Недійсний URL для GitHub.");
        }

        public async Task DownloadFilesAsync(List<Ukrainizer> ukrainizers, TableLayoutPanel uaList, string gameNamesArray = null)
        {
            string[] gameNames = gameNamesArray.Split(';');
            List<Task> downloadTasks = new List<Task>();
            foreach (var ukrainizer in ukrainizers)
            {
                if (gameNamesArray == null || gameNames.Contains(ukrainizer.Title))
                {
                    UaBlock uaBlock = FindBlockById(uaList, ukrainizer.Id);
                    if (uaBlock != null)
                    {
                        uaBlock.enabledButtons(false);
                        downloadTasks.Add(DownloadFileAsync(ukrainizer, uaBlock.GetProgressBar()));
                    }
                }
            }

            try
            {
                await Task.WhenAll(downloadTasks);
                foreach (var ukrainizer in ukrainizers)
                {
                    UaBlock uaBlock = FindBlockById(uaList, ukrainizer.Id);
                    if (uaBlock != null)
                    {
                        uaBlock.enabledButtons(true);
                        uaBlock.UpdateLastUpdate(true);
                    }
                }
                if (gameNamesArray == null)
                {
                    MessageBox.Show("Оновлення завершено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час оновлення перекладу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task DownloadFileAsync(Ukrainizer _ukrainizer, (ProgressBar progressBar, Label percentLabel) block)
        {
            (string url, string path, string metaInfo, DateTime Last) = _ukrainizer.GetUpdateInfo();
            try
            {
                if (url.Contains("github.com"))
                {
                    DateTime newMetaInfo = await GetLatestCommitDateFromGitHub(url);
                    if (string.IsNullOrEmpty(metaInfo))
                    {
                        _ukrainizer.MetaInfo = newMetaInfo.ToString();
                    }
                    else if (!(newMetaInfo > DateTime.Parse(metaInfo)))
                    {
                        MessageBox.Show("Зараз актуальний переклад.", "Оновлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

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
                            throw new Exception("Не вдалося знайти посилання для підтвердження або файл.");
                        }
                    }

                    response.EnsureSuccessStatusCode();

                    string fileName = GetFileNameFromUrlOrResponse(url, response);
                    string fullPath = Path.Combine(path, fileName);

                    // візуалізація завантаження
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

                    string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                    switch (fileExtension)
                    {
                        case ".zip":
                        case ".rar":
                        case ".7z":
                        case ".tar":
                        case ".gz":
                            ExtractArchiveFile(fullPath, path);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час оновлення перекладу.\nПосилання: {url}\nПомилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ExtractArchiveFile(string archiveFilePath, string extractPath)
        {
            try
            {
                using (var archive = ArchiveFactory.Open(archiveFilePath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            string destinationPath = Path.Combine(extractPath, entry.Key);
                            if (File.Exists(destinationPath))
                            {
                                File.Delete(destinationPath);
                            }

                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                            entry.WriteToFile(destinationPath, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                        }
                    }
                }
                File.Delete(archiveFilePath);

                string settingsFilePath = Path.Combine(extractPath, "Updater.txt");
                if (File.Exists(settingsFilePath))
                {
                    ProcessSettingsFile(settingsFilePath, extractPath);
                    File.Delete(settingsFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час розпакування архіву: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessSettingsFile(string settingsFilePath, string baseExtractPath)
        {
            try
            {
                var settings = File.ReadAllLines(settingsFilePath);

                foreach (var line in settings)
                {
                    if (string.IsNullOrWhiteSpace(line) || !line.Contains(":"))
                        continue;

                    string[] parts = line.Split(new[] { ':' }, 2);
                    string sourceFile = parts[0].Trim();
                    string destinationFolder = parts[1].Trim().Trim('"', '\'');

                    string sourcePath = Path.Combine(baseExtractPath, sourceFile);
                    string destinationPath = Path.GetFullPath(Path.Combine(baseExtractPath, destinationFolder));
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    if (File.Exists(sourcePath))
                    {
                        destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));

                        if (File.Exists(destinationPath))
                        {
                            File.Delete(destinationPath);
                        }
                        File.Move(sourcePath, destinationPath);
                    }
                    else if (Directory.Exists(sourcePath))
                    {
                        destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));
                        CopyDirectoryWithReplace(sourcePath, destinationPath);
                        Directory.Delete(sourcePath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час обробки файлу налаштувань: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopyDirectoryWithReplace(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string targetFilePath = Path.Combine(targetDir, Path.GetFileName(filePath));
                File.Copy(filePath, targetFilePath, true);
            }

            foreach (string directoryPath in Directory.GetDirectories(sourceDir))
            {
                string targetDirectoryPath = Path.Combine(targetDir, Path.GetFileName(directoryPath));
                CopyDirectoryWithReplace(directoryPath, targetDirectoryPath);
            }
        }
    }
}
