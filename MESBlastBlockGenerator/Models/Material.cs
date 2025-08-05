using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class Material
    {
        [XmlAttribute("material_code")]
        public string MaterialCode { get; set; }

        [XmlAttribute("material_shortname")]
        public string MaterialShortName { get; set; }

        [XmlAttribute("QuantityCartridgePacked")]
        public int QuantityCartridgePacked { get; set; }

        [XmlAttribute("amount_eom")]
        public string AmountEom { get; set; }

        [XmlAttribute("is_explosive")]
        public bool IsExplosive { get; set; }

        [XmlAttribute("material_density")]
        public decimal MaterialDensity { get; set; }

        [XmlAttribute("cup_density")]
        public decimal CupDensity { get; set; }

        [XmlArray("amounts")]
        [XmlArrayItem("amount")]
        public List<Amount> Amounts { get; set; }
    }
}