using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using MESBlastBlockGenerator.Services.Interfaces;
using NLog;

namespace MESBlastBlockGenerator.Services
{
    public class JsonSerializationService : IJsonSerializationService
    {
        private readonly JsonSerializerOptions _options;
        private readonly Logger _logger;

        public JsonSerializationService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true
            };
            _logger = LogManager.GetCurrentClassLogger();
        }

        public T DeserializeFromFile<T>(string filePath) where T : class, new()
        {
            try
            {
                var json = File.ReadAllText(filePath);
                return Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка десериализации из файла {filePath}");
                return new T();
            }
        }

        public void SerializeToFile<T>(T obj, string filePath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                var json = JsonSerializer.Serialize(obj, _options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка сериализации в файл {filePath}");
                throw;
            }
        }

        public T Deserialize<T>(string json) where T : class, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return new T();

                return JsonSerializer.Deserialize<T>(json, _options) ?? new T();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка десериализации JSON");
                return new T();
            }
        }
    }
}