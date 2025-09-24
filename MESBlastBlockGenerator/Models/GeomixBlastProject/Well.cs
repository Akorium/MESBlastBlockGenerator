using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    public class Well
    {
        [XmlAttribute("WelID")]
        public string WellID { get; set; } = string.Empty;
        [XmlAttribute("WelNumber")]
        public string WellNumber { get; set; } = string.Empty;
        [XmlAttribute("Depth")]
        public string Depth { get; set; } = string.Empty;
        [XmlAttribute("X")]
        public string X { get; set; } = string.Empty;
        [XmlAttribute("Y")]
        public string Y { get; set; } = string.Empty;
        [XmlAttribute("Z")]
        public string Z { get; set; } = string.Empty;
        [XmlAttribute("DX")]
        public string DX { get; set; } = "0";
        [XmlAttribute("DY")]
        public string DY { get; set; } = "0";
        [XmlAttribute("DM")]
        public string DM { get; set; } = "0";
        [XmlAttribute("RigID")]
        public string RigID { get; set; } = string.Empty;
        [XmlAttribute("DriverID")]
        public string DriverID { get; set; } = string.Empty;
        [XmlElement("Charges")]
        public Charges Charges { get; set; } = new();
    }
}