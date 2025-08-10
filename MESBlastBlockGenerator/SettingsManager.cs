using MESBlastBlockGenerator.Models;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MESBlastBlockGenerator
{
    public static class SettingsManager
    {
        private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                   "MESBlastGenerator", "settings.json");
        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, };

        public static AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json);
                }
            }
            catch { /* Логирование ошибки */ }

            return new AppSettings();
        }
        public static void SaveSettings(AppSettings settings)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                var json = JsonSerializer.Serialize(settings, jsonSerializerOptions);
                File.WriteAllText(SettingsPath, json);
            }
            catch { /* Логирование ошибки */ }
        }
    }
}
