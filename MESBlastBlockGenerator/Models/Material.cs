using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class Material
    {
        [XmlAttribute("material_code")]
        public string MaterialCode { get; set; } = "798031";

        [XmlAttribute("material_shortname")]
        public string MaterialShortName { get; set; } = "Шашка-детонатор литая ПТ-П-750";

        [XmlAttribute("QuantityCartridgePacked")]
        public string QuantityCartridgePacked { get; set; } = "0";

        [XmlAttribute("amount_eom")]
        public string AmountEom { get; set; } = "кг";

        [XmlAttribute("is_explosive")]
        public string IsExplosive { get; set; } = "false";

        [XmlAttribute("material_density")]
        public string MaterialDensity { get; set; } = "1200";

        [XmlAttribute("cup_density")]
        public string CupDensity { get; set; } = string.Empty;

        [XmlArray("amounts")]
        [XmlArrayItem("amount")]
        public List<Amount> Amounts { get; set; } = [new()];
    }
}