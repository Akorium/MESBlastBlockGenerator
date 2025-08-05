using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class SoapXmlRequest
    {
        [XmlElement(ElementName = "xmlRequest", Namespace = "http://tempuri.org/")]
        public XmlRequest XmlRequest { get; set; }
    }
}