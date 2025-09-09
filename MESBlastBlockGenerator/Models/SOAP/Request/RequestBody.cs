using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP.Request
{
    public class RequestBody
    {
        [XmlElement(ElementName = "SoapXmlRequest", Namespace = "http://tempuri.org/")]
        public SoapXmlRequest SoapXmlRequest { get; set; } = new SoapXmlRequest();
    }
}