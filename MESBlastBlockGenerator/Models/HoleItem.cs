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
        public string HoleTypeCode { get; set; }

        [XmlAttribute("hole_material")]
        public string HoleMaterial { get; set; }

        [XmlAttribute("hole_material_code")]
        public string HoleMaterialCode { get; set; }

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
        public decimal PlannedSubdrill { get; set; }

        [XmlAttribute("ExplosiveRatioByWell")]
        public decimal ExplosiveRatioByWell { get; set; }

        [XmlAttribute("depth_plan")]
        public decimal DepthPlan { get; set; }

        [XmlAttribute("depth_plan_eom_id")]
        public string DepthPlanEomId { get; set; }

        [XmlAttribute("depth_plan_eom")]
        public string DepthPlanEom { get; set; }

        [XmlAttribute("depth_fact")]
        public decimal DepthFact { get; set; }

        [XmlAttribute("depth_fact_eom_id")]
        public string DepthFactEomId { get; set; }

        [XmlAttribute("depth_fact_eom")]
        public string DepthFactEom { get; set; }

        [XmlAttribute("diameter_plan")]
        public decimal DiameterPlan { get; set; }

        [XmlAttribute("diameter_eom_id")]
        public string DiameterEomId { get; set; }

        [XmlAttribute("diameter_eom")]
        public string DiameterEom { get; set; }

        [XmlAttribute("diameter_fact")]
        public decimal DiameterFact { get; set; }

        [XmlAttribute("diameter_fact_eom_id")]
        public string DiameterFactEomId { get; set; }

        [XmlAttribute("diameter_fact_eom")]
        public string DiameterFactEom { get; set; }

        [XmlAttribute("x")]
        public decimal X { get; set; }

        [XmlAttribute("y")]
        public decimal Y { get; set; }

        [XmlAttribute("z")]
        public decimal Z { get; set; }

        [XmlAttribute("x_fact")]
        public decimal XFact { get; set; }

        [XmlAttribute("y_fact")]
        public decimal YFact { get; set; }

        [XmlAttribute("z_fact")]
        public decimal ZFact { get; set; }

        [XmlAttribute("isDrilling")]
        public bool IsDrilling { get; set; }

        [XmlAttribute("isDefective")]
        public bool IsDefective { get; set; }

        [XmlAttribute("isDelete")]
        public bool IsDelete { get; set; }
    }

}