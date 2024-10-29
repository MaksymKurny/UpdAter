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
        public List<Ukrainizer> GetUaInList()
        {
            return List.Where(u => u.AddToList).ToList();
        }
        public (List<Ukrainizer>, List<Ukrainizer>) GetPinnedList()
        {
            var pinnedUkrainizers = List.Where(u => u.PinnedState).ToList();
            var unpinnedUkrainizers = List.Where(u => !u.PinnedState).ToList();
            return (pinnedUkrainizers, unpinnedUkrainizers);
        }
        public void ChangeAddToList(string id, bool isChecked)
        {
            Ukrainizer uaToChange = List.FirstOrDefault(u => u.Id == id);
            if (uaToChange != null)
            {
                uaToChange.ChangeAddToList(isChecked);
            }
        }
        public void DellUkrainizer(string id)
        {
            Ukrainizer uaToRemove = List.FirstOrDefault(u => u.Id == id);
            if (uaToRemove != null) List.Remove(uaToRemove);
        }

        public void ChangePinnedState(string id, bool isChecked)
        {
            Ukrainizer uaToChange = List.FirstOrDefault(u => u.Id == id);
            if (uaToChange != null)
            {
                uaToChange.ChangePinnedState(isChecked);
            }
        }
    }

    public class Ukrainizer
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string GuideUrl { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string Banner { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool AddToList { get; set; }
        public bool PinnedState { get; set; }
        public string MetaInfo { get; set; }

        public Ukrainizer() {
            Id = Guid.NewGuid().ToString();
        }

        public void SetData((string title, string path, string url, 
            string icon, string banner, string guideUrl, string metaInfo, DateTime lastUpdate, bool addToList, bool pinnedState) data)
        {
            Title = data.title;
            Url = data.url;
            Path = data.path;
            Icon = data.icon;
            Banner = data.banner;
            GuideUrl = data.guideUrl;
            MetaInfo = data.metaInfo;
            LastUpdate = data.lastUpdate;
            AddToList = data.addToList;
            PinnedState = data.pinnedState;
        }
        public void SetShortData((string title, string url, string path, string icon, string banner, string guideUrl) data)
        {
            Title = data.title;
            Url = data.url;
            Path = data.path;
            Icon = data.icon;
            Banner = data.banner;
            GuideUrl = data.guideUrl;
        }
        public void UpdateLastUpdate(DateTime lastUpdate)
        {
            LastUpdate = lastUpdate;
        }
        public void ChangeAddToList(bool isChecked)
        {
            AddToList = isChecked;
        }
        internal void ChangePinnedState(bool isChecked)
        {
            PinnedState = isChecked;
        }
        public (string, string, string, DateTime) GetUpdateInfo()
        {
            return (Url, Path, MetaInfo, LastUpdate);
        }
        public (string, string, string, string, string, string, string, string, DateTime, bool, bool) GetFullData()
        {
            return (Id, Title, Url, Path, Icon, Banner, GuideUrl, MetaInfo, LastUpdate, AddToList, PinnedState);
        }
    }
}
