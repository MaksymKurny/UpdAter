using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace UpdAter.DL
{
    public static class FileHandler
    {
        public static void SaveSettingsToFile(string filePath, BL.Ukrainizers workspace)
        {
            // Серіалізація об'єкту workspace в JSON
            string json = JsonConvert.SerializeObject(workspace, Formatting.Indented);

            // Збереження JSON у файл
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public static BL.Ukrainizers OpenSettingsFromFile(string filePath)
        {
            try
            {
                // Зчитування JSON з файлу за вказаним шляхом
                string json = File.ReadAllText(filePath, Encoding.UTF8);

                // Десеріалізація JSON
                return JsonConvert.DeserializeObject<BL.Ukrainizers>(json);
            }
            catch (Exception ex)
            {
                // Обробка помилок під час зчитування або десеріалізації
                throw new Exception($"Помилка при відкритті налаштувань '{filePath}': {ex.Message}");
            }
        }
    }
}
