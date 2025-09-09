using MESBlastBlockGenerator.Models.SOAP.Request;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope<TBody> where TBody : class
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public object Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public TBody Body { get; set; }
    }
}
