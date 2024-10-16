﻿using System;
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

        public async Task DownloadFilesAsync(List<Ukrainizer> ukrainizers, TableLayoutPanel uaList)
        {
            List<Task> downloadTasks = new List<Task>();
            foreach (var ukrainizer in ukrainizers)
            {   
                UaBlock uaBlock = FindBlockById(uaList, ukrainizer.Id);
                if (uaBlock != null)
                {
                    uaBlock.enabledButtons(false);
                    downloadTasks.Add(DownloadFileAsync(ukrainizer.Url, ukrainizer.Path, uaBlock.GetProgressBar()));
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
            foreach (var ukrainizer in ukrainizers)
            {
                if (gameNames.Contains(ukrainizer.Title))
                {
                    UaBlock uaBlock = FindBlockById(uaList, ukrainizer.Id);
                    if (uaBlock != null)
                    {
                        uaBlock.enabledButtons(false);
                        downloadTasks.Add(DownloadFileAsync(ukrainizer.Url, ukrainizer.Path, uaBlock.GetProgressBar()));
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
                            throw new Exception("Не вдалося знайти посилання для підтвердження або файл.");
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
