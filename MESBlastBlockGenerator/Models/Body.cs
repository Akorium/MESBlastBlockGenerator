using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class Body
    {
        [XmlElement(ElementName = "SoapXmlRequest", Namespace = "http://tempuri.org/")]
        public SoapXmlRequest SoapXmlRequest { get; set; }
    }
}