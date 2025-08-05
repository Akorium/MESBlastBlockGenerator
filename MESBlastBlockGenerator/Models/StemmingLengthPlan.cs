using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class StemmingLengthPlan
    {
        [XmlAttribute("value")]
        public string Value { get; set; } = "4.59";
    }
}