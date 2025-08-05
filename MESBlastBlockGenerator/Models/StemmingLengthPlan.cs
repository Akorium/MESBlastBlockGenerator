using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class StemmingLengthPlan
    {
        [XmlAttribute("value")]
        public decimal Value { get; set; }
    }
}