using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class HoleItem
    {
        [XmlAttribute("blast_project_Id")]
        public string BlastProjectId { get; set; }

        [XmlAttribute("hole_id")]
        public string HoleId { get; set; }

        [XmlAttribute("hole_number")]
        public string HoleNumber { get; set; }

        [XmlAttribute("hole_type_code")]
        public string HoleTypeCode { get; set; } = "Explosive";

        [XmlAttribute("hole_material")]
        public string HoleMaterial { get; set; } = "Взрывные скважины ВСДП";

        [XmlAttribute("hole_material_code")]
        public string HoleMaterialCode { get; set; } = "1078066";

        [XmlAttribute("pit_code")]
        public string PitCode { get; set; }

        [XmlAttribute("pit_name")]
        public string PitName { get; set; }

        [XmlAttribute("level_code")]
        public string LevelCode { get; set; }

        [XmlAttribute("level_name")]
        public string LevelName { get; set; }

        [XmlAttribute("block_code")]
        public string BlockCode { get; set; }

        [XmlAttribute("block_name")]
        public string BlockName { get; set; }

        [XmlAttribute("blockDrilling_code")]
        public string BlockDrillingCode { get; set; }

        [XmlAttribute("blockDrilling_name")]
        public string BlockDrillingName { get; set; }

        [XmlAttribute("blockBlasting_code")]
        public string BlockBlastingCode { get; set; }

        [XmlAttribute("blockBlasting_name")]
        public string BlockBlastingName { get; set; }

        [XmlAttribute("PlannedSubdrill")]
        public string PlannedSubdrill { get; set; } = "1";

        [XmlAttribute("ExplosiveRatioByWell")]
        public string ExplosiveRatioByWell { get; set; } = "1.252";

        [XmlAttribute("depth_plan")]
        public string DepthPlan { get; set; } = "9.5";

        [XmlAttribute("depth_plan_eom_id")]
        public string DepthPlanEomId { get; set; } = "006";

        [XmlAttribute("depth_plan_eom")]
        public string DepthPlanEom { get; set; } = "м";

        [XmlAttribute("depth_fact")]
        public string DepthFact { get; set; } = "7";

        [XmlAttribute("depth_fact_eom_id")]
        public string DepthFactEomId { get; set; } = "018";

        [XmlAttribute("depth_fact_eom")]
        public string DepthFactEom { get; set; } = "пог. м";

        [XmlAttribute("diameter_plan")]
        public string DiameterPlan { get; set; } = "233";

        [XmlAttribute("diameter_eom_id")]
        public string DiameterEomId { get; set; } = "004";

        [XmlAttribute("diameter_eom")]
        public string DiameterEom { get; set; } = "см";

        [XmlAttribute("diameter_fact")]
        public string DiameterFact { get; set; } = "233";

        [XmlAttribute("diameter_fact_eom_id")]
        public string DiameterFactEomId { get; set; } = "003";

        [XmlAttribute("diameter_fact_eom")]
        public string DiameterFactEom { get; set; } = "мм";

        [XmlAttribute("x")]
        public string X { get; set; }

        [XmlAttribute("y")]
        public string Y { get; set; }

        [XmlAttribute("z")]
        public string Z { get; set; } = "980.66";

        [XmlAttribute("x_fact")]
        public string XFact { get; set; }

        [XmlAttribute("y_fact")]
        public string YFact { get; set; }

        [XmlAttribute("z_fact")]
        public string ZFact { get; set; } = "980.66";

        [XmlAttribute("isDrilling")]
        public string IsDrilling { get; set; } = "true";

        [XmlAttribute("isDefective")]
        public string IsDefective { get; set; } = "false";

        [XmlAttribute("isDelete")]
        public string IsDelete { get; set; } = "false";
    }

}