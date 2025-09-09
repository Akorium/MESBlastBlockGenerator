using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP.Response
{
    public class XmlResponse
    {
        [XmlElement(ElementName = "AsuSzmInSoapResponseDto", Namespace = "http://tempuri.org/")]
        public AsuSzmInSoapResponseDto AsuSzmInSoapResponseDto { get; set; } = new AsuSzmInSoapResponseDto();
    }
}