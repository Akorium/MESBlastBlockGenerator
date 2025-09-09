using MESBlastBlockGenerator.Helpers;
using MESBlastBlockGenerator.Models;
using MESBlastBlockGenerator.Models.BlastProject;
using MESBlastBlockGenerator.Models.SOAP;
using MESBlastBlockGenerator.Models.SOAP.Request;
using MESBlastBlockGenerator.Services.Interfaces;
using NLog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Services
{
    public class XmlGenerationService(IXmlSerializationService serializationService) : IXmlGenerationService
    {
        private readonly IXmlSerializationService _serializationService = serializationService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly XmlSerializerNamespaces _soapNamespaces = new(
            [
            new XmlQualifiedName("x", "http://schemas.xmlsoap.org/soap/envelope/"),
            new XmlQualifiedName("tem", "http://tempuri.org/")
            ]);

        public async Task<string> GenerateXmlContentAsync(InputParameters inputs)
        {
            return await Task.Run(() =>
            {
                //Форматирование самого сообщения и SOAP-конверта отличаются, поэтому мы их сериализуем отдельно
                var mesPmv = MesPmvMessageGenerationHelper.GenerateMesPmvMessage(inputs);
                _logger.Debug("Сообщение mesPmv сгенерировано");
                var innerXml = _serializationService.Serialize(mesPmv, new XmlSerializerNamespaces());
                var envelope = new Envelope<RequestBody>
                {
                    Header = new object(),
                    Body = new RequestBody
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
                return _serializationService.Serialize(envelope, _soapNamespaces);
            });
        }
    }
}
