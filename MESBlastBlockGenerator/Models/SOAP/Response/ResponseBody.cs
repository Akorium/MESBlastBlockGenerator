using MESBlastBlockGenerator.Models.SOAP.Request;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.SOAP.Response
{
    public class ResponseBody
    {
        [XmlElement(ElementName = "SoapXmlRequestResponse", Namespace = "http://tempuri.org/")]
        public SoapXmlRequestResponse SoapXmlRequestResponse { get; set; } = new SoapXmlRequestResponse();
    }
}
