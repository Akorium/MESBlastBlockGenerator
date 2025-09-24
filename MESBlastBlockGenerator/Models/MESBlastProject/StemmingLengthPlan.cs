using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.BlastProject
{
    public class StemmingLengthPlan
    {
        [XmlAttribute("value")]
        public string Value { get; set; } = "4.59";
    }
}