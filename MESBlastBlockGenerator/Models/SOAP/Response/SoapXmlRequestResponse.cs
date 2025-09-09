using MESBlastBlockGenerator.Models.SOAP.Request;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP.Response
{
    public class SoapXmlRequestResponse
    {
        [XmlElement(ElementName = "xmlResponse", Namespace = "http://tempuri.org/")]
        public XmlResponse XmlResponse { get; set; } = new XmlResponse();
    }
}