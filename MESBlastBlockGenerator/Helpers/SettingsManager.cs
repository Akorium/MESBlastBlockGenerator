using MESBlastBlockGenerator.Models;
using MESBlastBlockGenerator.Models.Settings;
using MESBlastBlockGenerator.Services;
using MESBlastBlockGenerator.Services.Interfaces;
using NLog;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MESBlastBlockGenerator.Helpers
{
    public static class SettingsManager
    {
        private static readonly string _appSettingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MESBlastGenerator", "appsettings.json");
        private static readonly string _userInputsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MESBlastGenerator", "InputParameters.json");
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly JsonSerializationService _serializationService = new();

        public static InputParameters LoadUserInputs()
        {
            try
            {
                if (!File.Exists(_userInputsPath))
                {
                    _logger.Warn($"Файл введённых данных не найден");
                    InputParameters inputParameters = new();
                    _serializationService.SerializeToFile(inputParameters, _userInputsPath);
                    return inputParameters;
                }
                return _serializationService.DeserializeFromFile<InputParameters>(_userInputsPath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка загрузки введённых данных");
                return new InputParameters();
            }
        }

        public static void SaveUserInputs(InputParameters settings)
        {
            try
            {
                _serializationService.SerializeToFile(settings, _userInputsPath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка сохранения введённых данных");
                throw;
            }
        }

        public static AppSettings LoadAppSettings()
        {
            try
            {
                if (!File.Exists(_appSettingsPath))
                {
                    _logger.Warn($"Файл конфигурации не найден");
                    AppSettings appSettings = new();
                    _serializationService.SerializeToFile(appSettings, _appSettingsPath);
                    return appSettings;
                }
                return _serializationService.DeserializeFromFile<AppSettings>(_appSettingsPath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка загрузки настроек");
                return new AppSettings();
            }
        }
    }
}
