using System;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace UpdAter
{
    static class Program
    {
        private const string repoOwner = "MaksymKurny";
        private const string repoName = "UpdAter";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool hiddenMode = args.Contains("--hidden");
            MainForm mainForm = new MainForm(args);
            if (hiddenMode)
            {
                mainForm.WindowState = FormWindowState.Minimized;
                mainForm.ShowInTaskbar = false;
                mainForm.Visible = false;
                Application.Run();
            }
            else
            {
                _ = CheckForUpdates();
                Application.Run(mainForm);
            }
        }
        static async Task CheckForUpdates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
                    client.Timeout = TimeSpan.FromSeconds(10);

                    HttpResponseMessage response = await client.GetAsync($"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest");
                    response.EnsureSuccessStatusCode();

                    string json = response.Content.ReadAsStringAsync().Result;
                    dynamic info = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    string latestVersion = info.tag_name?.ToString() ?? "0.0.0";
                    string downloadUrl = info.html_url?.ToString();
                    string releaseNotes = info.body?.ToString() ?? "Немає доступних нотаток про оновлення.";

                    string localVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    if (IsNewVersionAvailable(localVersion, latestVersion))
                    {
                        string message = $"Доступна нова версія: {latestVersion}!\n\nЩо нового:\n{releaseNotes}\n\nБажаєте завантажити?";

                        DialogResult result = MessageBox.Show(
                            message,
                            "Оновлення",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Information
                        );

                        if (result == DialogResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = downloadUrl,
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Запит до GitHub API був скасований через таймаут.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при перевірці оновлення: {ex.Message}");
            }
        }

        static bool IsNewVersionAvailable(string local, string latest)
        {
            Version localVer = new Version(local);
            Version latestVer = new Version(latest.TrimStart('v'));
            return latestVer > localVer;
        }
    }
}
