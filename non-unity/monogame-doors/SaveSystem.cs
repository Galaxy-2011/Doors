using System;
using System.IO;
using System.Text.Json;

namespace MonogameDoors
{
    public static class SaveSystem
    {
        private static string SavePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "monogame_doors_save.json");

        public static void Save(SaveData data)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(SavePath, json);
                Console.WriteLine($"Saved to {SavePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
            }
        }

        public static SaveData? Load()
        {
            try
            {
                if (!File.Exists(SavePath)) return null;
                var json = File.ReadAllText(SavePath);
                var data = JsonSerializer.Deserialize<SaveData>(json);
                Console.WriteLine($"Loaded save from {SavePath}");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
                return null;
            }
        }
    }
}
