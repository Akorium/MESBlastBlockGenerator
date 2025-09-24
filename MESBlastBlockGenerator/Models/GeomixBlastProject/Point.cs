using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    public class Point
    {
        [XmlAttribute("X")]
        public string X { get; set; } = string.Empty;
        [XmlAttribute("Y")]
        public string Y { get; set; } = string.Empty;
        [XmlAttribute("Z")]
        public string Z { get; set; } = string.Empty;
    }
}