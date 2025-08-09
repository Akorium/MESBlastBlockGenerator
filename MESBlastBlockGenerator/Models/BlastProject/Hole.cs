using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.BlastProject
{
    public class Hole
    {
        [XmlElement(ElementName = "holeitem")]
        public required HoleItem HoleItem { get; set; }

        [XmlArray("planChargeMaterials")]
        [XmlArrayItem("material")]
        public required List<Material> PlanChargeMaterials { get; set; }

        [XmlElement(ElementName = "stemming_length_plan")]
        public StemmingLengthPlan StemmingLengthPlan { get; set; } = new();
    }
}
