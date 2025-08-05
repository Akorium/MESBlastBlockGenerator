using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP
{
    public class XmlRequest
    {
        [XmlElement(ElementName = "Message", Namespace = "http://tempuri.org/")]
        public Message Message { get; set; }
    }
}