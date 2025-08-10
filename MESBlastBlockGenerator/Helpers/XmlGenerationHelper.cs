using MESBlastBlockGenerator.DTO;
using MESBlastBlockGenerator.Models.BlastProject;
using MESBlastBlockGenerator.Models.SOAP;
using NLog;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Helpers
{
    public class XmlGenerationHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly XmlSerializer _mesPmvSerializer = new(typeof(MesPmv));
        private static readonly XmlSerializer _envelopeSerializer = new(typeof(Envelope));
        private static readonly XmlSerializerNamespaces _soapNamespaces = new(
            [
            new XmlQualifiedName("x", "http://schemas.xmlsoap.org/soap/envelope/"),
            new XmlQualifiedName("tem", "http://tempuri.org/")
            ]);
        
        private static readonly XmlWriterSettings xmlSettings = new()
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            OmitXmlDeclaration = true
        };

        /// <summary>
        /// Генерирует XML передаваемый MES.
        /// </summary>
        /// <param name="inputs">Объект типа InputParameters</param>
        /// <returns>Сгенерированное содержимое XML в виде строки.</returns>
        public static string GenerateXmlContent(InputParameters inputs)
        {
            //Форматирование самого сообщения и SOAP-конверта отличаются, поэтому мы их сериализуем отдельно
            var mesPmv = MesPmvMessageGenerationHelper.GenerateMesPmvMessage(inputs);
            logger.Debug("Сообщение mesPmv сгенерировано");
            var innerXml = Serialize(mesPmv, new XmlSerializerNamespaces());
            var envelope = new Envelope
            {
                Body = new Body
                {
                    SoapXmlRequest = new SoapXmlRequest
                    {
                        XmlRequest = new XmlRequest
                        {
                            Message = new Message { XmlContent = innerXml }
                        }
                    }
                }
            };
            return Serialize(envelope, _soapNamespaces);
        }

        /// <summary>
        /// Сериализует объект в строку XML с использованием предоставленных XmlSerializerNamespaces.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который необходимо сериализовать.</typeparam>
        /// <param name="obj">Объект, который необходимо сериализовать.</param>
        /// <param name="ns">XmlSerializerNamespaces, используемые для сериализации.</param>
        /// <returns>Сериализованная строка XML.</returns>
        private static string Serialize<T>(T obj, XmlSerializerNamespaces ns)
        {
            using var stream = new StringWriter();
            using var writer = XmlWriter.Create(stream, xmlSettings);

            var serializer = GetSerializer(typeof(T));
            serializer.Serialize(writer, obj, ns);

            return stream.ToString();
        }
        /// <summary>
        /// Возвращает экземпляр XmlSerializer на основе указанного типа.
        /// </summary>
        /// <param name="type">Тип, для которого необходимо получить XmlSerializer.</param>
        /// <returns>Экземпляр XmlSerializer для указанного типа.</returns>
        private static XmlSerializer GetSerializer(Type type)
        {
            return type == typeof(MesPmv) ? _mesPmvSerializer
                 : type == typeof(Envelope) ? _envelopeSerializer
                 : throw new NotImplementedException();
        }
    }
}
