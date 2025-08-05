using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class Amount
    {
        [XmlAttribute("value")]
        public decimal Value { get; set; }

        [XmlAttribute("priority")]
        public int Priority { get; set; }
    }
}