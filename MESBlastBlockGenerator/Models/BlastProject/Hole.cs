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
        public List<Material> PlanChargeMaterials { get; set; } = 
            [
                new(), 
                new Material 
                { 
                    MaterialCode = "1025160",
                    MaterialShortName = "Вещество взрывчатое Березит Э-70",
                    IsExplosive = "true",
                    MaterialDensity = "1200",
                    CupDensity = "0",
                    Amounts = 
                    [
                        new Amount
                        {
                            Value = "500",
                            Priority = "1"
                        },
                        new Amount
                        {
                            Value = "600",
                            Priority = "2"
                        }
                    ]
                }
            ];

        [XmlElement(ElementName = "stemming_length_plan")]
        public StemmingLengthPlan StemmingLengthPlan { get; set; } = new();
    }
}
