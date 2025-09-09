using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP.Request
{
    public class XmlRequest
    {
        [XmlElement(ElementName = "Message", Namespace = "http://tempuri.org/")]
        public Message Message { get; set; } = new Message();
    }
}