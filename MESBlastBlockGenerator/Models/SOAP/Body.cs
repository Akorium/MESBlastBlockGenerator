using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP
{
    public class Body
    {
        [XmlElement(ElementName = "SoapXmlRequest", Namespace = "http://tempuri.org/")]
        public SoapXmlRequest SoapXmlRequest { get; set; }
    }
}