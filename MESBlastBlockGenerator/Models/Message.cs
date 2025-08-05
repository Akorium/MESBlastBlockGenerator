using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class Message
    {
        [XmlElement(ElementName = "mes_pmv")]
        public MesPmv MesPmv { get; set; }
    }
}