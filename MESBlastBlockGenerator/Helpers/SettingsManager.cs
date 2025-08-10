using MESBlastBlockGenerator.Models;
using NLog;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MESBlastBlockGenerator.Helpers
{
    public static class SettingsManager
    {
        private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                   "MESBlastGenerator", "settings.json");
        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, };
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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
            catch (Exception ex)
            {
                logger.Error(ex, "Ошибка загрузки настроек");
                throw;
            }

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
            catch (Exception ex)
            {
                logger.Error(ex, "Ошибка сохранения настроек");
                throw;
            }
        }
    }
}
