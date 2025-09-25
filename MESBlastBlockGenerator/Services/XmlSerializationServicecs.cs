using MESBlastBlockGenerator.Services.Interfaces;
using NLog;
using SkiaSharp;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Services
{
    public class XmlSerializationService : IXmlSerializationService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly XmlWriterSettings _xmlSettings = new()
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            OmitXmlDeclaration = true
        };

        public string Serialize<T>(T obj, XmlSerializerNamespaces? namespaces = null)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var writer = XmlWriter.Create(memoryStream, _xmlSettings);

                var serializer = GetSerializer(typeof(T));
                namespaces ??= new XmlSerializerNamespaces();
                serializer.Serialize(writer, obj, namespaces);
                writer.Flush();

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка сериализации объекта типа {typeof(T).Name}");
                throw;
            }
        }

        public T Deserialize<T>(string xmlContent) where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    _logger.Warn("Попытка десериализации пустого XML контента");
                    return null;
                }

                var serializer = GetSerializer(typeof(T));
                using var reader = new StringReader(xmlContent);
                return (T)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка десериализации XML контента в тип {typeof(T).Name}");
                throw;
            }
        }

        public XmlSerializer GetSerializer(Type type)
        {
            return new XmlSerializer(type);
        }
    }
}