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
                   "MESBlastGenerator", "settings.json");
        private static readonly JsonSerializerOptions _jsonSerializerOptions = 
            new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, };
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка загрузки настроек");
            }

            return new AppSettings();
        }
        public static void SaveSettings(AppSettings settings)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath));
                var json = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка сохранения настроек");
                throw;
            }
        }

        internal static void UpdateAndSaveSettings(AppSettings appSettings, InputParameters inputs)
        {
            UpdateSettings(appSettings, inputs);
            SaveSettings(appSettings);
        }

        private static void UpdateSettings(AppSettings appSettings, InputParameters inputs)
        {
            appSettings.MaxRow = inputs.MaxRow.ToString();
            appSettings.MaxCol = inputs.MaxCol.ToString();
            appSettings.RotationAngle = inputs.RotationAngle.ToString();
            appSettings.BaseX = inputs.BaseX.ToString();
            appSettings.BaseY = inputs.BaseY.ToString();
            appSettings.BaseZ = inputs.BaseZ.ToString();
            appSettings.Distance = inputs.Distance.ToString();
            appSettings.PitName = inputs.PitName.ToString();
            appSettings.Level = inputs.Level.ToString();
            appSettings.BlockNumber = inputs.BlockNumber.ToString();
            appSettings.DesignDepth = inputs.DesignDepth.ToString();
            appSettings.RealDepth = inputs.RealDepth.ToString();
            appSettings.DispersedCharge = inputs.DispersedCharge;
            appSettings.MainChargeMass = inputs.MainChargeMass.ToString();
            if (inputs.DispersedCharge)
                appSettings.SecondaryChargeMass = inputs.SecondaryChargeMass.ToString();
            appSettings.StemmingLength = inputs.StemmingLength.ToString();
        }
    }
}
