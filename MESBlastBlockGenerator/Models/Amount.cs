using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class Amount
    {
        [XmlAttribute("value")]
        public string Value { get; set; } = "0.75";

        [XmlAttribute("priority")]
        public string Priority { get; set; } = "1";
    }
}