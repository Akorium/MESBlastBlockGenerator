using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP.Response
{
    public class AsuSzmInSoapResponseDto
    {
        [XmlElement(ElementName = "Error", Namespace = "http://tempuri.org/")]
        public string Error { get; set; } = string.Empty;

        [XmlElement(ElementName = "Status", Namespace = "http://tempuri.org/")]
        public string Status { get; set; } = string.Empty;
    }
}