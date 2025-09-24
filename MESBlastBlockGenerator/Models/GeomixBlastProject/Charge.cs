using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    public class Charge
    {
        [XmlAttribute("Q")]
        public string Quantity { get; set; } = string.Empty;

        [XmlAttribute("L")]
        public string Length { get; set; } = string.Empty;

        [XmlAttribute("E")]
        public string ExplosiveType { get; set; } = "Гранулит М";

        [XmlAttribute("B")]
        public string BoosterType { get; set; } = "Патронит М-60";

        [XmlAttribute("B1")]
        public string BoosterType2 { get; set; } = "Патронит М-60";

        [XmlAttribute("D")]
        public string DetonatorType { get; set; } = "Искра - Ш";

        [XmlAttribute("DL")]
        public string Delay { get; set; } = "0";

        [XmlAttribute("D1")]
        public string DetonatorType2 { get; set; } = "";

        [XmlAttribute("DL1")]
        public string Delay2 { get; set; } = "0";
    }
}