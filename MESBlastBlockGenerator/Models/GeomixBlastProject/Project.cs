using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    public class Project
    {
        [XmlAttribute("ProjectID")]
        public string ProjectID { get; set; } = string.Empty;
        [XmlAttribute("DateBegin")]
        public string DateBegin { get; set; } = string.Empty;
        [XmlAttribute("DateEnd")]
        public string DateEnd { get; set; } = string.Empty;
        [XmlElement("Blocks")]
        public Blocks Blocks { get; set; } = new Blocks();
    }
}