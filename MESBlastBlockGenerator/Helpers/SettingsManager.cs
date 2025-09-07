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
        private static readonly string _settingsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                   "MESBlastGenerator", "InputParameters.json");
        private static readonly JsonSerializerOptions _jsonSerializerOptions = 
            new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, };
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static InputParameters LoadSavedInputs()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    return JsonSerializer.Deserialize<InputParameters>(json);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка загрузки настроек");
            }

            return new InputParameters();
        }
        public static void SaveInputs(InputParameters inputs)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath));
                var json = JsonSerializer.Serialize(inputs, _jsonSerializerOptions);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка сохранения настроек");
                throw;
            }
        }
    }
}
