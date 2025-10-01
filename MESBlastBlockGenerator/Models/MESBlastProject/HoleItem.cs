using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.MESBlastProject
{
    public class HoleItem
    {
        [XmlAttribute("blast_project_Id")]
        public required string BlastProjectId { get; set; }

        [XmlAttribute("hole_id")]
        public required string HoleId { get; set; }

        [XmlAttribute("hole_number")]
        public required string HoleNumber { get; set; }

        [XmlAttribute("hole_type_code")]
        public string HoleTypeCode { get; set; } = "Explosive";

        [XmlAttribute("hole_material")]
        public string HoleMaterial { get; set; } = "";

        [XmlAttribute("hole_material_code")]
        public string HoleMaterialCode { get; set; } = "";

        [XmlAttribute("pit_code")]
        public required string PitCode { get; set; }

        [XmlAttribute("pit_name")]
        public required string PitName { get; set; }

        [XmlAttribute("level_code")]
        public required string LevelCode { get; set; }

        [XmlAttribute("level_name")]
        public required string LevelName { get; set; }

        [XmlAttribute("block_code")]
        public required string BlockCode { get; set; }

        [XmlAttribute("block_name")]
        public required string BlockName { get; set; }

        [XmlAttribute("blockDrilling_code")]
        public required string BlockDrillingCode { get; set; }

        [XmlAttribute("blockDrilling_name")]
        public required string BlockDrillingName { get; set; }

        [XmlAttribute("blockBlasting_code")]
        public required string BlockBlastingCode { get; set; }

        [XmlAttribute("blockBlasting_name")]
        public required string BlockBlastingName { get; set; }

        [XmlAttribute("PlannedSubdrill")]
        public string PlannedSubdrill { get; set; } = "1";

        [XmlAttribute("ExplosiveRatioByWell")]
        public string ExplosiveRatioByWell { get; set; } = "1.252";

        [XmlAttribute("depth_plan")]
        public required string DepthPlan { get; set; }

        [XmlAttribute("depth_plan_eom_id")]
        public string DepthPlanEomId { get; set; } = "006";

        [XmlAttribute("depth_plan_eom")]
        public string DepthPlanEom { get; set; } = "м";

        [XmlAttribute("depth_fact")]
        public string? DepthFact { get; set; } = null;

        [XmlAttribute("depth_fact_eom_id")]
        public string? DepthFactEomId { get; set; } = null;

        [XmlAttribute("depth_fact_eom")]
        public string? DepthFactEom { get; set; } = null;

        [XmlAttribute("diameter_plan")]
        public required string DiameterPlan { get; set; }

        [XmlAttribute("diameter_eom_id")]
        public string DiameterEomId { get; set; } = "004";

        [XmlAttribute("diameter_eom")]
        public string DiameterEom { get; set; } = "мм";

        [XmlAttribute("diameter_fact")]
        public string? DiameterFact { get; set; } = null;

        [XmlAttribute("diameter_fact_eom_id")]
        public string? DiameterFactEomId { get; set; } = null;

        [XmlAttribute("diameter_fact_eom")]
        public string? DiameterFactEom { get; set; } = null;

        [XmlAttribute("x")]
        public required string X { get; set; }

        [XmlAttribute("y")]
        public required string Y { get; set; }

        [XmlAttribute("z")]
        public required string Z { get; set; }

        [XmlAttribute("x_fact")]
        public string? XFact { get; set; } = null;

        [XmlAttribute("y_fact")]
        public string? YFact { get; set; } = null;

        [XmlAttribute("z_fact")]
        public string? ZFact { get; set; } = null;

        [XmlAttribute("isDrilling")]
        public required string IsDrilling { get; set; }

        [XmlAttribute("isDefective")]
        public string IsDefective { get; set; } = "false";

        [XmlAttribute("isDelete")]
        public string IsDelete { get; set; } = "false";
    }

}