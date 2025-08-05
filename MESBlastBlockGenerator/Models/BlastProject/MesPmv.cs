using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.BlastProject
{
    [XmlRoot(ElementName = "mes_pmv", Namespace = "")]
    public class MesPmv
    {
        [XmlAttribute("messageid")]
        public string MessageId { get; set; } = "1022a282f6afb23b0f3b";

        [XmlAttribute("systemid")]
        public string SystemId { get; set; } = "MES";

        [XmlAttribute("businessid")]
        public string BusinessId { get; set; } = string.Empty;

        [XmlElement(ElementName = "holes_in_blast_project")]
        public required HolesInBlastProject HolesInBlastProject { get; set; }
    }
}