using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class XmlRequest
    {
        [XmlElement(ElementName = "Message", Namespace = "http://tempuri.org/")]
        public Message Message { get; set; }
    }
}