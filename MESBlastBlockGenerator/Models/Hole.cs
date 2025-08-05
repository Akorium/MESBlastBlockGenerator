using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class Hole
    {
        [XmlElement(ElementName = "holeitem")]
        public required HoleItem HoleItem { get; set; }

        [XmlArray("planChargeMaterials")]
        [XmlArrayItem("material")]
        public List<Material> PlanChargeMaterials { get; set; } = new List<Material>();

        [XmlElement(ElementName = "stemming_length_plan")]
        public StemmingLengthPlan StemmingLengthPlan { get; set; } = new StemmingLengthPlan();
    }
}
