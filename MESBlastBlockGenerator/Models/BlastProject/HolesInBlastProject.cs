using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.BlastProject
{
    public class HolesInBlastProject
    {
        [XmlElement(ElementName = "hole")]
        public required List<Hole> Holes { get; set; }
    }
}