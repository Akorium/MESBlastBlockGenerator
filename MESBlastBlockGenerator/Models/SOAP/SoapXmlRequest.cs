using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP
{
    public class SoapXmlRequest
    {
        [XmlElement(ElementName = "xmlRequest", Namespace = "http://tempuri.org/")]
        public XmlRequest XmlRequest { get; set; }
    }
}