using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UpdAter.DL;

namespace UpdAter.BL
{
    public class Bl
    {
        public Ukrainizers ukrainizers;
        private string settingsName = "settings.json";

        public Bl()
        {
            ukrainizers = new Ukrainizers();
            LoadSettings();
        }

        public void LoadSettings()
        {
            if (!File.Exists(settingsName)) return;
            openSettingsByPath(settingsName);
        }

        public void saveSettings()
        {
            try
            {
                string filePath = settingsName;

                if (!string.IsNullOrEmpty(filePath))
                {
                    DL.FileHandler.SaveSettingsToFile(filePath, ukrainizers);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження робочого простору: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openSettingsByPath(string filePath)
        {
            try
            {
                // Виклик методу з FileHandler для відкриття простору за вказаним шляхом
                ukrainizers = FileHandler.OpenSettingsFromFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class Ukrainizers
    {
        public List<Ukrainizer> List { get; set; }

        public Ukrainizers()
        {
            List = new List<Ukrainizer>();
        }

        public void UpdateUkrainizer(int index, (string, string, string, string, string, DateTime) data)
        {
            List[index].SetData(data);
        }
        public void UpdateUkrainizerDate(int index, DateTime lastUpdate)
        {
            List[index].UpdateLastUpdate(lastUpdate);
        }
        public void DellNewUkrainizer()
        {
            List.Remove(List[0]);
        }
        public void DellUkrainizer(string title, string url)
        {
            Ukrainizer uaToRemove = List.FirstOrDefault(n => n.Title == title && n.Url == url);
            if (uaToRemove != null) List.Remove(uaToRemove);
        }
    }

    public class Ukrainizer
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string Banner { get; set; }
        public string MetaInfo { get; set; }
        public DateTime LastUpdate { get; set; }

        public Ukrainizer(){}

        public void SetData((string title, string path, string url, string icon, string banner, DateTime lastUpdate) data)
        {
            Title = data.title;
            Path = data.path;
            Url = data.url;
            Icon = data.icon;
            Banner = data.banner;
            LastUpdate = data.lastUpdate;
        }
        public void UpdateLastUpdate(DateTime lastUpdate)
        {
            LastUpdate = lastUpdate;
        }
        public (string, string, string, string, string, DateTime) GetData()
        {
            return (Title, Url, Path, Icon, Banner, LastUpdate);
        }
    }
}
